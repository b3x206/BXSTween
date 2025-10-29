#if UNITY_5_6_OR_NEWER
using BX.Tweening.Interop;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BX.Tweening
{
    /// <summary>
    /// The primary runner for the Unity Game Engine.
    /// <br>Initializes the <see cref="BXSTween"/> by a <see cref="RuntimeInitializeOnLoadMethodAttribute"/> method.</br>
    /// </summary>
    public class BXSTweenMBRunner : MonoBehaviour, IBXSTweenLoop
    {
        private readonly List<BXSTweenable> m_RunningTweens = new List<BXSTweenable>(1024);
        public List<BXSTweenable> RunningTweens => m_RunningTweens;
        private readonly BXSTweenTaskDeferrer<BXSTweenable> m_TaskDeferrer = new BXSTweenTaskDeferrer<BXSTweenable>();
        public BXSTweenTaskDeferrer<BXSTweenable> TaskDeferrer => m_TaskDeferrer;
        private readonly IBXSTweenLogger m_Logger = new BXSTweenMBLogger();
        public IBXSTweenLogger Logger => m_Logger;

        // Depending on the current frame, provide the delta time depending on the current frame and cache it (use 'lastFrameDeltaTime')
        // > This is because there's a 1ms access penalty for accessing Time.deltaTime
        // > on higher tween quantities (for some reason..) (note that this is a debug deep profile build)
        public int ElapsedTickCount => Time.frameCount;
        private float m_PreviousUnscaledDeltaTime = 0f;
        public float UnscaledDeltaTime => m_PreviousUnscaledDeltaTime;

        // This also has performance penalty, but this one is okay as a tween MAY change Time.timeScale
        // Still stupid that this takes 2ms with a 10k sample size (with deep profiling on) though..
        public float TimeScale => Time.timeScale;

        public bool SupportsFixedTick => true;
        private float m_PreviousUnscaledFixedDeltaTime = 0f;
        public float FixedUnscaledDeltaTime => m_PreviousUnscaledFixedDeltaTime;

        public event Action<IBXSTweenLoop> OnInit;
        public event Action<IBXSTweenLoop> OnTick;
        public event Action<IBXSTweenLoop> OnFixedTick;
        public event IBXSTweenLoop.ExitAction OnExit;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void OnApplicationLoad()
        {
            if (BXSTween.NeedsInitialization)
            {
                // Initialize the static things
                BXSTween.Initialize(() =>
                {
                    // Spawn object
                    BXSTweenMBRunner runner = new GameObject("BXSTween").AddComponent<BXSTweenMBRunner>();
                    runner.OnInit?.Invoke(runner);

                    // Add it to 'DontDestroyOnLoad'
                    DontDestroyOnLoad(runner.gameObject);

                    // Return it as this is the getter if the main IBXSTweenRunner is null
                    return runner;
                });
            }
        }

        public void Kill()
        {
            OnExit?.Invoke(this, false);
            Destroy(gameObject);
        }

        private void Update()
        {
            m_PreviousUnscaledDeltaTime = Time.unscaledDeltaTime;
            OnTick?.Invoke(this);
        }
        private void FixedUpdate()
        {
            m_PreviousUnscaledFixedDeltaTime = Time.fixedUnscaledDeltaTime;
            OnFixedTick?.Invoke(this);
        }

        private void OnApplicationQuit()
        {
            OnExit?.Invoke(this, true);
        }
    }
}
#endif
