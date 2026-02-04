using BX.Tweening.Collections;
using BX.Tweening.Events;
using BX.Tweening.Interop;
using BX.Tweening.Utility;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace BX.Tweening
{
    /// <summary>
    /// The core code for BXSTween. This class manages:
    /// <br>* The primary core loop <see cref="CurrentLoop"/> and global loop <see cref="GlobalLoop"/></br>
    /// <br>* Behaviour for <see cref="IBXSTweenLoop"/> to simplify the interface implementation.</br>
    /// <br>* * Utility for management of tweens of certain <see cref="IBXSTweenLoop"/></br>
    /// <br/>
    /// <br>* May contain shorthands for common UnityEngine namespace objects.</br>
    /// </summary>
    public static partial class BXSTween
    {
        // -- Prepare
#if !UNITY_5_6_OR_NEWER || ENABLE_MONO
        static BXSTween()
        {
            // Works better if I have a sizeable `static readonly` value on the given class.
            Type bxTwEaseType = typeof(BXSTweenEase);
            RuntimeHelpers.RunClassConstructor(bxTwEaseType.TypeHandle);
        }
#endif
        // -- Runtime
        private static readonly object m_GlobalCreateLock = new object();
        private static IBXSTweenLoop m_Global;
        // This is a pretty noticeable slowdown, if .Value is gathered constantly..
        // While each IBXSTweenLoop callback uses self as the callback param, avoiding this as hotpath, the editor script doesn't
        // ^ The editor script now caches the result of "AsyncLocal<T>.Current".
        //   This is stored in the ExecutionContext per async task (and our current global instance), which is a list of globals.
        private static readonly AsyncLocal<IBXSTweenLoop> m_Current = new AsyncLocal<IBXSTweenLoop>();

        /// <summary>
        /// Action used to create a <see cref="IBXSTweenLoop"/>.
        /// <br>This is set when the BXSTween is nulled for some reason.</br>
        /// </summary>
        private static BXSGetterAction<IBXSTweenLoop> m_GetMainRunnerAction;

        /// <summary>
        /// Get the global BXSTween loop
        /// <br>This is the only loop you use if you have unity.</br>
        /// </summary>
        public static IBXSTweenLoop GlobalLoop
        {
            get
            {
                if (m_Global == null)
                {
                    lock (m_GlobalCreateLock)
                    {
                        if (m_GetMainRunnerAction == null)
                        {
                            throw new InvalidOperationException($"[BXSTween::(get)GlobalLoop] Failed to get global loop before initialization.");
                        }

                        m_Global = m_GetMainRunnerAction();

                        // This theoretically shouldn't be a problem unless you are doing something async in a MonoBehaviour ctor.
                        // But even then, using "Use" should fix this and ignore this value as this is null coalesence assigned
                        // (and UnityEngine objects on a seperate ExecutionContext will be null null, not weakref null)
                        m_Current.Value ??= m_Global;
                    }
                }

                return m_Global;
            }
        }
        /// <summary>
        /// Get the current loop
        /// <br><b>You shouldn't be getting this value constantly on a hotpath. It should be cached if required and otherwise 
        /// the callback parameter of <see cref="IBXSTweenLoop"/> should be used.</b></br>
        /// </summary>
        public static IBXSTweenLoop CurrentLoop => m_Current.Value ?? GlobalLoop;

        /// <summary>
        /// Use a temporary <see cref="IBXSTweenLoop"/> for <see cref="CurrentLoop"/>
        /// <br>This only sets the <see cref="CurrentLoop"/> and does not set the global loop.</br>
        /// </summary>
        /// <returns>A loop scope to use and dispose.</returns>
        /// <exception cref="ArgumentNullException"/>
        public static IDisposable UseLoop(IBXSTweenLoop loop)
        {
            if (loop == null)
            {
                throw new ArgumentNullException(nameof(loop));
            }

            return new LoopScope(loop);
        }

        private sealed class LoopScope : IDisposable
        {
            private bool disposed = false;
            private readonly IBXSTweenLoop previous;
            public LoopScope(IBXSTweenLoop loop)
            {
                previous = m_Current.Value;
                m_Current.Value = loop;
            }
            public void Dispose()
            {
                if (disposed)
                {
                    // or silently handle it and don't??
                    throw new InvalidOperationException("[BXSTween::LoopScope::Dispose] Failed to dispose the IBXSTweenLoop scope more than once.");
                }

                m_Current.Value = previous;
                disposed = true;
            }
        }

        /// <summary>
        /// Whether if the BXSTween needs it's initial <see cref="Initialize(BXSGetterAction{IBXSTweenLoop})"/> called.
        /// <br>After calling <see cref="Initialize(BXSGetterAction{IBXSTweenLoop})"/> once will make this false.</br>
        /// </summary>
        public static bool NeedsInitialization => m_GetMainRunnerAction == null;

        /// <summary>
        /// Whether to ensure the tweens to be removed from the <see cref="RunningTweens"/> list on <see cref="BXSTweenable.Stop"/>.
        /// <br>Use this if you get warnings like 'Non playing tween "..." tried to be run' like errors.</br>
        /// <br/>
        /// <br>
        /// This hack was only necessary in the beginning of the development, 
        /// it is usually not necessary nowadays unless you play the tween when it stops for some reason.
        /// </br>
        /// </summary>
        public static bool EnsureTweenRemovalOnStop = false;

        private static BXSEqualityComparison<object> m_LinkObjectComparison = BXSTweenCompare.Equals;
        /// <summary>
        /// Comparison callback for <see cref="BXSTweenable.LinkObject"/> validity,
        /// when the <see cref="BXSTweenable"/> has a link object.
        /// <br>This value is never <see langword="null"/>.</br>
        /// </summary>
        public static BXSEqualityComparison<object> LinkObjectComparison
        {
            get
            {
                m_LinkObjectComparison ??= BXSTweenCompare.Equals;
                return m_LinkObjectComparison;
            }
            set
            {
                m_LinkObjectComparison = value ?? throw new ArgumentNullException(nameof(value));
            }
        }
        /// <summary>
        /// Epsilon for <see cref="BXSTweenable.Speed"/> and <see cref="IBXSTweenLoop.TimeScale"/> during time drain.
        /// </summary>
        private const float TweenDrainEpsilon = 1e-8f;

        /// <summary>
        /// Initializes the <see cref="IBXSTweenLoop"/> with <paramref name="getRunnerAction"/>.
        /// <br>This initializes the <see cref="GlobalLoop"/>. To replace the <see cref="CurrentLoop"/> use <see cref="UseLoop(IBXSTweenLoop)"/>.</br>
        /// </summary>
        /// <param name="getRunnerAction">Create the runner in this method. This is called for lazy initialization when needed.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Initialize(BXSGetterAction<IBXSTweenLoop> getRunnerAction)
        {
            if (getRunnerAction == null)
            {
                throw new ArgumentNullException(nameof(getRunnerAction), "[BXSTween::Initialize] Given parameter is null.");
            }

            // Call this first to call all events on the 'OnRunnerExit'
            m_Global?.Kill();

            // Hook the tween runner
            m_Global = getRunnerAction();
            m_GetMainRunnerAction = getRunnerAction;
            m_Current.Value = m_Global;
            HookTweenLoop(m_Current.Value);
        }
        // - Batch Management
        /// <summary>
        /// Find all <b>running tweens</b> by tag.
        /// </summary>
        /// <param name="loop">Tween loop to search in it's <see cref="IBXSTweenLoop.RunningTweens"/>.</param>
        /// <param name="tag">Tag to look for. This can be anything, including <see langword="null"/> and <see cref="string.Empty"/></param>
        /// <returns>Collection of tweens with the tag. May return an empty collection (but a non-null value) if there are none.</returns>
        public static BXSTweenableCollection FindByTag(IBXSTweenLoop loop, string tag)
        {
            BXSTweenableCollection result = new BXSTweenableCollection(tag);

            for (int i = (loop.RunningTweens.Count - 1); i >= 0; i--)
            {
                var tw = loop.RunningTweens[i];
                if (string.Equals(tw.Tag, tag, StringComparison.Ordinal))
                {
                    result.Add(tw);
                }
            }

            return result;
        }
        /// <inheritdoc cref="FindByTag(IBXSTweenLoop, string)"/>
        public static BXSTweenableCollection FindByTag(string tag)
        {
            return FindByTag(CurrentLoop, tag);
        }
        /// <summary>
        /// Stops all tweens of <paramref name="loop"/>.
        /// </summary>
        public static void Stop(IBXSTweenLoop loop)
        {
            for (int i = loop.RunningTweens.Count - 1; i >= 0; i--)
            {
                // This may fail if too many tweens were running (?)
                if (i >= loop.RunningTweens.Count)
                {
                    i -= (loop.RunningTweens.Count - (i + 1));
                    continue;
                }

                BXSTweenable tween = loop.RunningTweens[i];
                if (tween == null)
                {
                    continue;
                }

                // This is technically a 'RemoveAt' call.
                tween.Stop();
            }

            loop.RunningTweens.Clear();
        }
        /// <summary>
        /// Stops all tweens of <see cref="CurrentLoop"/>.
        /// </summary>
        public static void Stop()
        {
            Stop(CurrentLoop);
        }
        /// <summary>
        /// Clears the tweens of <paramref name="loop"/> and kills the loop.
        /// <br>This requires a reinitialization.</br>
        /// </summary>
        public static void Clear(IBXSTweenLoop loop)
        {
            Stop(loop);
            loop?.Kill();
        }
        /// <summary>
        /// Clears the tweens of <see cref="CurrentLoop"/> and kills the loop.
        /// <br>This requires a reinitialization.</br>
        /// </summary>
        public static void Clear()
        {
            Clear(CurrentLoop);
        }

        // -- Tweening
        /// <summary>
        /// Runs a tweenable.
        /// <br>The <paramref name="tween"/> itself contains the state.</br>
        /// </summary>
        public static void RunTweenable(IBXSTweenLoop loop, BXSTweenable tween)
        {
            // Checks
            if (!tween.IsValid)
            {
                loop.RunningTweens.Remove(tween);
                loop.Logger.Error($"[BXSTweenable::RunTweenable] Invalid tween '{tween}' tried to be run, stopping and removing it.");
                tween.LastRunFailed = true;
                tween.Stop();
                return;
            }
            if (!tween.IsPlaying)
            {
                loop.RunningTweens.Remove(tween);
                loop.Logger.Warn($"[BXSTweenable::RunTweenable] Non playing tween '{tween}' tried to be run. This is likely a BXSTween sided issue unless you managed tweens yourself.");
                return;
            }

            if (tween.IsInstant)
            {
                try
                {
                    tween.OnStartAction?.Invoke();
                    // tween.Stop already calls OnEndAction
                }
                catch (Exception e)
                {
                    loop.Logger.Exception($"[BXSTween::RunTweenable] OnStartAction+OnEndAction in tween '{tween}'\n", e);
                }
                tween.EvaluateTween(1f);
                tween.Stop();

                return;
            }

            // Tickability
            if (tween.TickConditionAction != null)
            {
                TickSuspendAction suspendType = TickSuspendAction.None;

                try
                {
                    suspendType = tween.TickConditionAction();
                }
                catch (Exception e)
                {
                    loop.Logger.Exception($"[BXSTween::RunTweenable] TickConditionAction in tween '{tween}'\n", e);
                    suspendType = TickSuspendAction.Stop;
                }

                switch (suspendType)
                {
                    case TickSuspendAction.Tick:
                        return;
                    case TickSuspendAction.Pause:
                        tween.Pause();
                        return;
                    case TickSuspendAction.Stop:
                        tween.Stop();
                        return;

                    default:
                    case TickSuspendAction.None:
                        break;
                }
            }

            // DeltaTime
            float deltaTime;
            switch (tween.ActualTickType)
            {
                default:
                case TickType.Variable:
                    deltaTime = loop.UnscaledDeltaTime;
                    break;
                case TickType.Fixed:
                    deltaTime = loop.FixedUnscaledDeltaTime;
                    break;
            }
            deltaTime *= ((tween.IgnoreTimeScale ? 1f : loop.TimeScale) * tween.Speed);

            // No time has passed
            if (deltaTime < TweenDrainEpsilon)
            {
                return;
            }

            // We **must** drain the delta time to simulate the tweens correctly.
            float remaining = deltaTime;
            // If the validity check isn't done once, make this true
            int loValidState = -1; // -1 = unchecked, 0 = false, 1 = true

            while (remaining > 0f)
            {
                bool isFirstRun = tween.LoopsElapsed == 0;

                // Not very redundant if the callbacks allow changing these.
                if (((tween.IgnoreTimeScale ? 1f : loop.TimeScale) * tween.Speed) <= TweenDrainEpsilon)
                {
                    break;
                }

                // Check if the link object is valid
                if (tween.LinkInvalidAction != TickSuspendAction.None)
                {
                    bool loValidInterface = loValidState == 1;
                    if (loValidState == -1)
                    {
                        if (tween.LinkObject is IBXSTweenLinkObject lo)
                        {
                            try
                            {
                                loValidInterface = lo.IsValid();

                                if (lo.CheckValidityOnce)
                                {
                                    loValidState = loValidInterface ? 1 : 0;
                                }
                            }
                            catch (Exception e)
                            {
                                loValidInterface = false;
                                loop.Logger.Exception($"[BXSTween::RunTweenable] On `tween.LinkObject as IBXSTweenLinkObject` in tween '{tween}'\n", e);
                            }
                        }
                        else
                        {
                            loValidInterface = true;
                        }
                    }

                    if (!loValidInterface || LinkObjectComparison(tween.LinkObject, null))
                    {
                        if (tween.LinkInvalidAction == TickSuspendAction.Tick)
                        {
                            // This will print billion times, maybe add another tier named "trace"?
                            // Eh, it's fine for now (tm)
                            // To avoid the string interp from occuring here, call this conditionally..
                            if (loop.Logger.LogVerbosity <= IBXSTweenLogger.Verbosity.Info)
                            {
                                loop.Logger.Info($"[BXSTween::RunTweenable] Tween '{tween}' is blank ticked because the link is invalid.");
                            }

                            break;
                        }
                        else if (tween.LinkInvalidAction == TickSuspendAction.Pause)
                        {
                            if (loop.Logger.LogVerbosity <= IBXSTweenLogger.Verbosity.Info)
                            {
                                loop.Logger.Info($"[BXSTween::RunTweenable] Tween '{tween}' is paused because the link is invalid.");
                            }

                            tween.Pause();
                            break;
                        }
                        else if (tween.LinkInvalidAction == TickSuspendAction.Stop)
                        {
                            if (loop.Logger.LogVerbosity <= IBXSTweenLogger.Verbosity.Info)
                            {
                                loop.Logger.Info($"[BXSTween::RunTweenable] Tween '{tween}' is stopped because the link is invalid.");
                            }

                            tween.Stop();
                            break;
                        }
                    }
                }

                // Delay
                if (tween.DelayElapsed < 1f)
                {
                    // Instant finish + wait one frame
                    if (tween.StartingDelay <= 0f)
                    {
                        tween.DelayElapsed = 1f;
                    }
                    else
                    {
                        // Elapse delay further
                        // (only elapse if the tween has delaying)
                        float timeToFinish = (1f - tween.DelayElapsed) * tween.StartingDelay;
                        float advance = Math.Min(remaining, timeToFinish);

                        tween.DelayElapsed += advance / tween.StartingDelay;
                        remaining -= advance;
                    }

                    if (tween.DelayElapsed >= 1f && isFirstRun)
                    {
                        try
                        {
                            tween.OnStartAction?.Invoke();
                        }
                        catch (Exception e)
                        {
                            loop.Logger.Exception($"[BXSTween::RunTweenable] OnStartAction in tween '{tween}'\n", e);
                        }
                    }
                }
                // Yield next deltaTime, if none is left for the next iteration.
                if (remaining <= 0f)
                {
                    break;
                }

                // Tweening + Elapsing
                if (tween.CurrentElapsed < 1f)
                {
                    try
                    {
                        tween.EvaluateTween(tween.CurrentElapsed);
                        tween.OnTickAction?.Invoke();
                    }
                    catch (Exception e)
                    {
                        loop.Logger.Exception($"[BXSTween::RunTweenable] EvaluateTween+OnTickAction in tween '{tween}'\n", e);
                        tween.Stop();
                        tween.LastRunFailed = true;
                        break;
                    }

                    float timeToFinish = (1f - tween.CurrentElapsed) * tween.StartingDuration;
                    float advance = Math.Min(remaining, timeToFinish);

                    tween.CurrentElapsed += advance / tween.StartingDuration;
                    remaining -= advance;
                }

                if (tween.CurrentElapsed >= 1f)
                {
                    // Base tweening ended, set 'tween.EvaluateTween' with 1f.
                    tween.EvaluateTween(1f);

                    // Looping (infinite loop if the 'StartingLoopCount' is less than 0
                    // StartingLoopCount should be greater than 0 or less than zero to be able to loop.
                    // Only do loops if there is still yet to do loops
                    if (tween.StartingLoopCount < 0 || tween.LoopsElapsed < tween.StartingLoopCount)
                    {
                        // 1 away from max value to avoid overflow
                        tween.LoopsElapsed = Math.Clamp(tween.LoopsElapsed + 1, 0, int.MaxValue - 1);

                        // Call this before just in case the parameters are changed
                        try
                        {
                            tween.OnLoopRepeatAction?.Invoke();
                        }
                        catch (Exception e)
                        {
                            loop.Logger.Exception($"[BXSTween::RunTweenable] OnRepeatAction in tween '{tween}'\n", e);
                        }

                        // Reset the base while looping
                        tween.ResetPreserveLoop();
                        if (tween.LoopType == LoopType.Yoyo)
                        {
                            tween.SwapTargetValues = !tween.SwapTargetValues;
                        }

                        continue;
                    }

                    // Stop (doesn't require DeltaTime accuracy)
                    try
                    {
                        tween.OnEndAction?.Invoke();
                    }
                    catch (Exception e)
                    {
                        loop.Logger.Exception($"[BXSTween::RunTweenable] OnEndAction in tween '{tween}'\n", e);
                    }

                    // deltaTime no longer relevant for this simulation
                    tween.Stop();
                    break;
                }
            }
        }

        #region Tick Loop Core
        /// <summary>
        /// Hooks the tween runner.
        /// </summary>
        private static void HookTweenLoop(IBXSTweenLoop runner)
        {
            runner.OnExit += OnTweenLoopExit;
            runner.OnTick += OnTweenLoopTick;

            if (runner.SupportsFixedTick)
            {
                runner.OnFixedTick += OnTweenLoopFixedTick;
            }
        }
        private static void OnTweenLoopTick(IBXSTweenLoop runner)
        {
            // Iterate all tweens
            for (int i = 0; i < runner.RunningTweens.Count; i++)
            {
                // Run those tweens (if the tick is suitable)
                BXSTweenable tween = runner.RunningTweens[i];

                if (tween.ActualTickType == TickType.Variable)
                {
                    RunTweenable(runner, tween);
                }
            }

            // Iterate all deferred actions
            runner.TaskDeferrer.Tick();
        }
        private static void OnTweenLoopFixedTick(IBXSTweenLoop runner)
        {
            // Iterate all tweens
            for (int i = 0; i < runner.RunningTweens.Count; i++)
            {
                // Run those tweens (if the tick is suitable)
                BXSTweenable tween = runner.RunningTweens[i];

                if (tween.ActualTickType == TickType.Fixed)
                {
                    RunTweenable(runner, tween);
                }
            }
        }
        /// <summary>
        /// The exit method for the <see cref="CurrentLoop"/>.
        /// </summary>
        private static void OnTweenLoopExit(IBXSTweenLoop runner, bool cleanup)
        {
            // If we are quitting app as well clear the BXSTween runnings.
            if (cleanup)
            {
                Stop(runner);
                m_GetMainRunnerAction = null;
            }
        }
        #endregion
    }
}
