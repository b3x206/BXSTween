using System;
#if UNITY_2017_1_OR_NEWER
using UnityEngine;
#endif
using BX.Tweening.Events;
using BX.Tweening.Interop;

namespace BX.Tweening
{
    /// <summary>
    /// Flags used to specify pre defined actions in play.
    /// <br>This provides convenient actions to do, that are usually set with 
    /// <see cref="BXSTweenContext{TValue}.SetPlayAction(BXSAction, ValueSetMode)"/></br>
    /// </summary>
    [Flags]
    public enum PlayFlags
    {
        None = 0,

        // Things done often in "SetPlayAction"
        /// <summary>
        /// This calls the getter to set initial value when <see cref="BXSTweenable.Play"/> is called, if a start value exists.
        /// </summary>
        SetStartValue = 1 << 0,

        // Events often cleared in "Play"
        /// <summary>
        /// This calls <see cref="BXSTweenable.ClearStopAction"/>, if the following tween has a callback that may effect an object's state.
        /// <br>This is the event you want to use, if calling <see cref="BXSTweenable.Play"/> will stop your tween and will call the stop callback.</br>
        /// </summary>
        ClearStopActions = 1 << 29,
        /// <summary>
        /// This calls <see cref="BXSTweenable.ClearEndAction"/>, if the following tween has a callback that may effect an object's state.
        /// <br>This is more unlikely to be used, as this event is only called when the tween ends without any intervention.</br>
        /// </summary>
        ClearEndActions = 1 << 30,
        /// <summary>
        /// This calls <see cref="BXSTweenable.ClearAllActions"/>, if the following tween has callbacks that may effect an object's state.
        /// </summary>
        ClearAllActions = 1 << 31,
    }

    /// <summary>
    /// Contains a typed context.
    /// <br>The actual setters are contained here, along with the other values.</br>
    /// <br>This context handles most of the type related things + <see cref="BXSTweenable"/> setters.</br>
    /// <br/>
    /// <br>
    /// Overriding classes should implement this as abstract + a public default constructor 
    /// if another constructor that isn't default was made.
    /// </br>
    /// </summary>
    [Serializable]
    public abstract class BXSTweenContext<TValue> : BXSTweenable
    {
        // -- Start/End value
        /// <summary>
        /// The current gathered starting value.
        /// </summary>
        public TValue StartValue { get; protected set; }
        /// <summary>
        /// The current gathered ending value.
        /// </summary>
        public TValue EndValue { get; protected set; }
        /// <summary>
        /// The current last value used in <see cref="EvaluateTween(float)"/> of this context.
        /// <br>
        /// This is the current interpolation value for the corresponding
        /// <see cref="BXSTweenable.CurrentElapsed"/> time between <see cref="StartValue"/> and <see cref="EndValue"/>.
        /// </br>
        /// <br>This value updates accordingly if <see cref="SetStartValue(TValue)"/> or <see cref="SetEndValue(TValue)"/> is called in any way.</br>
        /// </summary>
        public TValue CurrentValue { get; protected set; }

        /// <summary>
        /// The function to get the 'StartValue'.
        /// <br>Can be used in tween starting by calling <see cref="SetStartValue()"/> for getting new value to interpolate from.</br>
        /// </summary>
        public BXSGetterAction<TValue> GetterAction { get; protected set; }
        /// <summary>
        /// The setter action, called when the tweening is being done.
        /// </summary>
        public BXSSetterAction<TValue> SetterAction { get; protected set; }

        /// <summary>
        /// Play flags to use when this context is played.
        /// <br>By default, nothing is done. To set play flags use <see cref="SetPlayFlags(PlayFlags, ValueSetMode)"/></br>
        /// <br/>
        /// <br>Unlike directly calling <see cref="Play(PlayFlags)"/>, the flags aren't consumed.
        /// Calling <see cref="Play()"/> without any flags will use these flags.</br>
        /// </summary>
        public PlayFlags PlayFlags { get; protected set; } = PlayFlags.None;

        // -- Interpolation
        /// <summary>
        /// The linear interpolation method to override for the setter of this <typeparamref name="TValue"/> context.
        /// <br>This expects an unclamped interpolation action.</br>
        /// </summary>
        public abstract TValue Lerp(TValue a, TValue b, float time);

        // - Overrides
        /// <summary>
        /// Returns whether the tween context has a setter.
        /// <br>This may also return whether if the <see cref="StartValue"/> and <see cref="EndValue"/> is not null if <typeparamref name="TValue"/> is nullable.</br>
        /// </summary>
        public override bool IsValid =>
            SetterAction != null &&
            // check if struct or not, if not a struct check nulls
            (typeof(TValue).IsValueType || (StartValue != null && EndValue != null));

        /// <summary>
        /// The tick type of a sequence is always to be run at the variable mode.
        /// </summary>
        public override TickType ActualTickType => TickType.Variable;

        /// <summary>
        /// Evaluates the <see cref="SetterAction"/> with <see cref="Lerp"/>.
        /// </summary>
        public override void EvaluateTween(float t)
        {
            // Easing checks and stuff is done on 'EvaluateEasing'.
            float easedTime = EvaluateEasing(t);
            CurrentValue = Lerp(
                a: SwapTargetValues ? EndValue : StartValue,
                b: SwapTargetValues ? StartValue : EndValue,
                time: easedTime
            );

            SetterAction(CurrentValue);
        }

        // -- Methods
        public override void CopyFrom<T>(T tweenable)
        {
            base.CopyFrom(tweenable);
            BXSTweenContext<TValue> tweenableAsContext = tweenable as BXSTweenContext<TValue>;
            if (tweenableAsContext == null)
            {
                return;
            }

            StartValue = tweenableAsContext.StartValue;
            EndValue = tweenableAsContext.EndValue;
            GetterAction = tweenableAsContext.GetterAction;
            SetterAction = tweenableAsContext.SetterAction;
        }

        // - Operators
        public static implicit operator bool(BXSTweenContext<TValue> context)
        {
            return context.IsValid;
        }

        // - Daisy Chain Setters
        /// <summary>
        /// Sets up context. Do this if your context is not <see cref="IsValid"/>.
        /// <br/>
        /// <br><see cref="ArgumentNullException"/> = Thrown when any of these are null : 
        /// <paramref name="startValueGetter"/> or <paramref name="setter"/>.</br>
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public BXSTweenContext<TValue> SetupContext(BXSGetterAction<TValue> startValueGetter, TValue endValue, BXSSetterAction<TValue> setter)
        {
            SetStartValue(startValueGetter).SetEndValue(endValue).SetSetter(setter);
            return this;
        }
        /// <summary>
        /// Sets up context.
        /// <br>A shortcut method for setting the setter and the start+end values.</br>
        /// </summary>
        public BXSTweenContext<TValue> SetupContext(TValue startValue, TValue endValue, BXSSetterAction<TValue> setter)
        {
            SetStartValue(startValue).SetEndValue(endValue).SetSetter(setter);
            return this;
        }

        /// <summary>
        /// Sets the start value from the getter <see cref="GetterAction"/>.
        /// <br>This can only be successfully called if there's a <see cref="GetterAction"/>, otherwise it will throw <see cref="NullReferenceException"/>.</br>
        /// </summary>
        /// <exception cref="NullReferenceException"/>
        public BXSTweenContext<TValue> SetStartValue()
        {
            if (GetterAction == null)
            {
                throw new NullReferenceException($"[BXSTweenContext<{typeof(TValue)}>::SetStartValue] Parameterless SetStartValue 'GetterAction()' value is null.");
            }

            return SetStartValue(GetterAction());
        }
        /// <summary>
        /// Sets the <see cref="GetterAction"/> value and sets the <see cref="StartValue"/> from it.
        /// </summary>
        /// <param name="getter">
        /// The getter to use. This value cannot be <see langword="null"/>.
        /// If you don't want to use a getter use the <see cref="SetStartValue(TValue)"/> without delegate and direct variable.
        /// <br>However, with a getter, you can use the parameterless <see cref="SetStartValue()"/> 
        /// to get a new value on demand (by hooking it up to events, etc.).</br>
        /// </param>
        /// <exception cref="ArgumentNullException"/>
        public BXSTweenContext<TValue> SetStartValue(BXSGetterAction<TValue> getter)
        {
            if (getter == null)
            {
                throw new ArgumentNullException(nameof(getter), $"[BXSTweenContext<{typeof(TValue)}>::SetStartValue] Given argument is null.");
            }

            GetterAction = getter;
            return SetStartValue(GetterAction());
        }
        /// <summary>
        /// Sets the starting value.
        /// <br>This effects the tween while running.</br>
        /// </summary>
        public BXSTweenContext<TValue> SetStartValue(TValue value)
        {
            StartValue = value;
            // Set the currently elapsed value to the lerp result.
            if (!IsPlaying)
            {
                CurrentValue = Lerp(StartValue, EndValue, EvaluateEasing(CurrentElapsed));
            }

            return this;
        }
        /// <inheritdoc cref="SetStartValue(BXSGetterAction{TValue})"/>
        public BXSTweenContext<TValue> SetGetter(BXSGetterAction<TValue> getter)
        {
            // alias, but specifies intent better.
            return SetStartValue(getter);
        }

        /// <summary>
        /// Sets the ending value.
        /// <br>This effects the tween while running.</br>
        /// </summary>
        /// <param name="setRelative">Whether to set the end value as a relative one. Calls <see cref="SetIsEndRelative(bool)"/>.</param>
        public BXSTweenContext<TValue> SetEndValue(TValue value)
        {
            EndValue = value;
            if (!IsPlaying)
            {
                CurrentValue = Lerp(StartValue, EndValue, EvaluateEasing(CurrentElapsed));
            }

            return this;
        }
        /// <summary>
        /// Sets the <see cref="SetterAction"/> value.
        /// </summary>
        /// <param name="setter">The setter. This value cannot be null.</param>
        /// <exception cref="ArgumentNullException"/>
        public BXSTweenContext<TValue> SetSetter(BXSSetterAction<TValue> setter)
        {
            if (setter == null)
            {
                throw new ArgumentNullException(nameof(setter), $"[BXSTweenContext<{typeof(TValue)}>::SetSetterAction] Given argument is null.");
            }

            SetterAction = setter;
            return this;
        }

        /// <summary>
        /// Sets the duration of the tween.
        /// <br>Has no effect after the tween was started.</br>
        /// </summary>
        public BXSTweenContext<TValue> SetDuration(float duration)
        {
            m_Duration = duration;
            return this;
        }
        /// <summary>
        /// Sets delay.
        /// <br>Has no effect after the tween was started.</br>
        /// </summary>
        /// <param name="delay">The delay to wait. Values equal or lower than 0 are no delay.</param>
        public BXSTweenContext<TValue> SetDelay(float delay)
        {
            // Calculate how much percent is the set delay (unless the delay was set to <= 0, which in that case set DelayElapsed to 1)
            if (IsPlaying && DelayElapsed < 1f)
            {
                // To not throw 'DivideByZeroException', make the epsilon way larger but a non-negligable value.
                if (delay <= 0.000001f)
                {
                    DelayElapsed = 1f;
                }
                else
                {
                    // Calculate how much of the delay remains while the delay is being ticked
                    DelayElapsed = Math.Min(1f, m_Delay / delay);
                }
            }

            m_Delay = delay;
            return this;
        }
        /// <summary>
        /// Sets the loop count.
        /// <br>Has no effect if the tween was started.</br>
        /// </summary>
        /// <param name="count">Count to set the tween. If this is lower than 0, the tween will loop forever.</param>
        public BXSTweenContext<TValue> SetLoopCount(int count)
        {
            m_LoopCount = count;
            return this;
        }
        /// <summary>
        /// Sets the loop type.
        /// <br>This does affect the tween after it was started.</br>
        /// </summary>
        /// <param name="type">Type of the loop. See <see cref="LoopType"/>'s notes for more information.</param>
        public BXSTweenContext<TValue> SetLoopType(LoopType type)
        {
            m_LoopType = type;

            return this;
        }
        /// <summary>
        /// Sets whether to wait the <see cref="BXSTweenable.Delay"/> when the tween repeats.
        /// </summary>
        public BXSTweenContext<TValue> SetWaitDelayOnLoop(bool doWait)
        {
            m_WaitDelayOnLoop = doWait;

            return this;
        }
        /// <summary>
        /// Sets the easing type.
        /// </summary>
        /// <param name="ease">The type of easing.</param>
        /// <param name="disableEaseCurve">Disables <see cref="BXSTweenable.UseEaseCurve"/>.</param>
        public BXSTweenContext<TValue> SetEase(EaseType ease, bool disableEaseCurve = false)
        {
            // This thing's setter already updates this value.
            Ease = ease;
            if (disableEaseCurve)
            {
                UseEaseCurve = false;
            }

            return this;
        }
#if UNITY_5_6_OR_NEWER
        /// <summary>
        /// Sets the easing curve.
        /// <br>
        /// Setting this null will disable <see cref="BXSTweenable.UseEaseCurve"/>, 
        /// setting it any non-null value will enable <see cref="BXSTweenable.UseEaseCurve"/>.
        /// </br>
        /// </summary>
        /// <param name="curve">The animation curve to set.</param>
        public BXSTweenContext<TValue> SetEaseCurve(AnimationCurve curve)
#else
        public BXSTweenContext<TValue> SetEaseCurve(IBXSTweenCurve curve)
#endif
        {
            m_EaseCurve = curve;
            m_UseEaseCurve = m_EaseCurve != null;

            return this;
        }
        /// <summary>
        /// Sets the speed of this tween.
        /// <br>Setting this value 0 or lower will make the tween not tick forward.</br>
        /// </summary>
        public BXSTweenContext<TValue> SetSpeed(float speed)
        {
            Speed = speed;

            return this;
        }
        /// <summary>
        /// Sets to whether to allow the <see cref="BXSTweenable.EvaluateEasing(float)"/> to overshoot.
        /// </summary>
        public BXSTweenContext<TValue> SetClampEasingSetter(bool doClamp)
        {
            m_Clamp01EasingSetter = doClamp;

            return this;
        }

        /// <summary>
        /// Set the ticking type of this tweenable.
        /// <br>This does affect what update the tween is running during playback.</br>
        /// </summary>
        /// <param name="type">Type of the update ticking. See <see cref="TickType"/>'s notes for more information.</param>
        public BXSTweenContext<TValue> SetTickType(TickType type)
        {
            m_TickType = type;

            return this;
        }
        /// <summary>
        /// Sets to whether ignore the time scale.
        /// <br>Setting this will run the tween unscaled except for it's <see cref="BXSTweenable.Speed"/>.</br>
        /// </summary>
        /// <param name="doIgnore"></param>
        public BXSTweenContext<TValue> SetIgnoreTimeScale(bool doIgnore)
        {
            m_IgnoreTimeScale = doIgnore;

            return this;
        }
        /// <summary>
        /// Unlinks the link object.
        /// </summary>
        public BXSTweenContext<TValue> SetLinkObjectNull()
        {
            m_LinkInvalidAction = TickSuspendAction.None;
            m_LinkObject = null;

            return this;
        }
        /// <summary>
        /// Sets the target object link.
        /// <br>With this, you can manage the tween behaviour when this link object dies.</br>
        /// </summary>
        /// <param name="invalidLinkAction">
        /// Suspend action done when the tween link is invalid.
        /// <br>The validity of this link is checked through the <see cref="BXSTween.LinkObjectComparison"/>, 
        /// which is set to <see cref="Utility.BXSTweenCompare.Equals"/> by default.</br>
        /// </param>
        public BXSTweenContext<TValue> SetLinkObject<T>(T obj, TickSuspendAction invalidLinkAction = TickSuspendAction.Stop) where T : class
        {
            if (BXSTween.LinkObjectComparison(obj, null))
            {
                m_LinkInvalidAction = invalidLinkAction;
                m_LinkObject = null;

                if (m_LinkInvalidAction == TickSuspendAction.Stop)
                {
                    Owner.Logger.Warn("[BXSTweenContext::SetLinkObject] Given link object is null but TickSuspendAction is Stop. If this is unintentional, please use SetLinkObjectNull instead.");
                }
            }

            m_LinkObject = obj;
            m_LinkInvalidAction = invalidLinkAction;

            return this;
        }
        /// <summary>
        /// Sets a <paramref name="tag"/> for this context. The tag value can be anything.
        /// </summary>
        public BXSTweenContext<TValue> SetTag(string tag)
        {
            Tag = tag;

            return this;
        }
        /// <summary>
        /// Set the value for <see cref="PlayFlags"/>, which specify what actions to do when <see cref="Play()"/> with no parameters are called.
        /// </summary>
        public BXSTweenContext<TValue> SetPlayFlags(PlayFlags flags, ValueSetMode mode = ValueSetMode.Equals)
        {
            switch (mode)
            {
                case ValueSetMode.Remove:
                    PlayFlags &= ~flags;
                    break;
                case ValueSetMode.Add:
                    PlayFlags |= flags;
                    break;

                default:
                case ValueSetMode.Equals:
                    PlayFlags = flags;
                    break;
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="BXSTweenable.OnPlayAction"/> event.
        /// <br>This is called when <see cref="BXSTweenable.Play"/> is called on this tween.</br>
        /// </summary>
        public BXSTweenContext<TValue> SetPlayAction(BXSAction action, ValueSetMode mode = ValueSetMode.Equals)
        {
            switch (mode)
            {
                case ValueSetMode.Remove:
                    OnPlayAction -= action;
                    break;
                case ValueSetMode.Add:
                    OnPlayAction += action;
                    break;

                default:
                case ValueSetMode.Equals:
                    OnPlayAction = action;
                    break;
            }
            return this;
        }
        /// <summary>
        /// Sets the <see cref="BXSTweenable.OnStartAction"/> event.
        /// <br>This is called when the tween has waited out it's delay and it is starting for the first time.</br>
        /// </summary>
        public BXSTweenContext<TValue> SetStartAction(BXSAction action, ValueSetMode mode = ValueSetMode.Equals)
        {
            switch (mode)
            {
                case ValueSetMode.Remove:
                    OnStartAction -= action;
                    break;
                case ValueSetMode.Add:
                    OnStartAction += action;
                    break;

                default:
                case ValueSetMode.Equals:
                    OnStartAction = action;
                    break;
            }
            return this;
        }
        /// <summary>
        /// Sets the <see cref="BXSTweenable.OnTickAction"/> event.
        /// <br>This is called every time the tween ticks. It is started to be called after the delay was waited out.</br>
        /// </summary>
        public BXSTweenContext<TValue> SetTickAction(BXSAction action, ValueSetMode mode = ValueSetMode.Equals)
        {
            switch (mode)
            {
                case ValueSetMode.Remove:
                    OnTickAction -= action;
                    break;
                case ValueSetMode.Add:
                    OnTickAction += action;
                    break;

                default:
                case ValueSetMode.Equals:
                    OnTickAction = action;
                    break;
            }
            return this;
        }
        /// <summary>
        /// Sets the <see cref="BXSTweenable.OnTickAction"/> event.<br/>
        /// This method is an alias for <see cref="SetTickAction(BXSAction, ValueSetMode)"/>.
        /// </summary>
        public BXSTweenContext<TValue> SetUpdateAction(BXSAction action, ValueSetMode mode = ValueSetMode.Equals)
        {
            return SetTickAction(action, mode);
        }
        /// <summary>
        /// Sets the <see cref="BXSTweenable.OnPauseAction"/> event.
        /// <br>It is called when <see cref="BXSTweenable.Pause"/> is called on this tween.</br>
        /// </summary>
        public BXSTweenContext<TValue> SetPauseAction(BXSAction action, ValueSetMode mode = ValueSetMode.Equals)
        {
            switch (mode)
            {
                case ValueSetMode.Remove:
                    OnPauseAction -= action;
                    break;
                case ValueSetMode.Add:
                    OnPauseAction += action;
                    break;

                default:
                case ValueSetMode.Equals:
                    OnPauseAction = action;
                    break;
            }
            return this;
        }
        /// <summary>
        /// Sets the <see cref="BXSTweenable.OnLoopRepeatAction"/> event.
        /// <br>This is called when the loop repeats.</br>
        /// </summary>
        public BXSTweenContext<TValue> SetLoopRepeatAction(BXSAction action, ValueSetMode mode = ValueSetMode.Equals)
        {
            switch (mode)
            {
                case ValueSetMode.Remove:
                    OnLoopRepeatAction -= action;
                    break;
                case ValueSetMode.Add:
                    OnLoopRepeatAction += action;
                    break;

                default:
                case ValueSetMode.Equals:
                    OnLoopRepeatAction = action;
                    break;
            }
            return this;
        }
        /// <summary>
        /// Sets the <see cref="BXSTweenable.OnEndAction"/> event.
        /// <br>The difference between the <see cref="SetStopAction(BXSAction, ValueSetMode)"/> 
        /// and this is that this only gets invoked when the tween ends after the tweens duration.</br>
        /// <br/>
        /// <br><b>Note : </b> If you are want to play the same tween from this tweens ending action use <see cref="SetStopAction(BXSAction, ValueSetMode)"/> instead,
        /// this is due to the <see cref="BXSTweenable.Stop"/> sets <see cref="BXSTweenable.IsPlaying"/> to false immediately after this event.</br>
        /// <br>Or use <see cref="BXSTweenable.PlayDelayed"/> to delay the playback by one frame.</br>
        /// <br>Or use <see cref="BXSTweenSequence"/> in conjuction with <see cref="BXSTweenable.AsCopy{T}"/>.</br>
        /// </summary>
        public BXSTweenContext<TValue> SetEndAction(BXSAction action, ValueSetMode mode = ValueSetMode.Equals)
        {
            switch (mode)
            {
                case ValueSetMode.Remove:
                    OnEndAction -= action;
                    break;
                case ValueSetMode.Add:
                    OnEndAction += action;
                    break;

                default:
                case ValueSetMode.Equals:
                    OnEndAction = action;
                    break;
            }
            return this;
        }
        /// <summary>
        /// Sets the <see cref="BXSTweenable.OnStopAction"/> event.
        /// <br>The difference between the <see cref="SetEndAction(BXSAction, ValueSetMode)"/>
        /// and this is that this gets called both when the tween ends or when <see cref="BXSTweenable.Stop"/> gets called.</br>
        /// </summary>
        public BXSTweenContext<TValue> SetStopAction(BXSAction action, ValueSetMode mode = ValueSetMode.Equals)
        {
            switch (mode)
            {
                case ValueSetMode.Remove:
                    OnStopAction -= action;
                    break;
                case ValueSetMode.Add:
                    OnStopAction += action;
                    break;

                default:
                case ValueSetMode.Equals:
                    OnStopAction = action;
                    break;
            }
            return this;
        }
        /// <summary>
        /// Sets the <see cref="BXSTweenable.TickConditionAction"/> action.
        /// <br>Return the suitable <see cref="TickSuspendAction"/> in the function.</br>
        /// </summary> 
        public BXSTweenContext<TValue> SetTickConditionAction(BXSTickConditionAction action, ValueSetMode mode = ValueSetMode.Equals)
        {
            switch (mode)
            {
                case ValueSetMode.Remove:
                    TickConditionAction -= action;
                    break;
                case ValueSetMode.Add:
                    TickConditionAction += action;
                    break;

                default:
                case ValueSetMode.Equals:
                    TickConditionAction = action;
                    break;
            }
            return this;
        }

        // -- State
        public override void Play()
        {
            Play(PlayFlags);
        }

        /// <inheritdoc cref="Play()"/>
        /// <param name="flags">Flags, for the predetermined actions to do when a tween is to be played.</param>
        public virtual void Play(PlayFlags flags)
        {
            if (!IsValid)
            {
                Owner.Logger.Error($"[BXSTweenContext::Play] This tweenable '{ToString()}' isn't valid. Cannot 'Play' tween.");
                LastRunFailed = true;
                return;
            }

            LastRunFailed = false;
            if ((flags & PlayFlags.SetStartValue) == PlayFlags.SetStartValue)
            {
                SetStartValue();
            }

            if ((flags & PlayFlags.ClearEndActions) == PlayFlags.ClearEndActions)
            {
                ClearEndAction();
            }
            if ((flags & PlayFlags.ClearStopActions) == PlayFlags.ClearStopActions)
            {
                ClearStopAction();
            }
            if ((flags & PlayFlags.ClearAllActions) == PlayFlags.ClearAllActions)
            {
                ClearAllActions();
            }

            base.Play();
        }
    }
}
