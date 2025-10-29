using System;
#if UNITY_5_6_OR_NEWER
using UnityEngine;
#else
using System.Numerics;
#endif

#if UNITY_5_6_OR_NEWER
using ExportAttribute = UnityEngine.SerializeField;
#else
using ExportAttribute = BX.Tweening.Interop.ExportStubAttribute;
#endif

namespace BX.Tweening
{
    /// <summary>
    /// Contains a context that uses Quaternion.
    /// </summary>
    [Serializable]
    public sealed class BXSTweenQuaternionContext : BXSTweenContext<Quaternion>
    {
        [Export]
        private bool m_UseSlerp = false;
        /// <summary>
        /// Use <see cref="Quaternion.Slerp"/> instead of Lerp.
        /// </summary>
        public bool UseSlerp => m_UseSlerp;

        public override Quaternion Lerp(Quaternion a, Quaternion b, float time)
        {
#if UNITY_5_6_OR_NEWER
            if (m_UseSlerp)
            {
                return Quaternion.SlerpUnclamped(a, b, time);
            }
            else
            {
                return Quaternion.LerpUnclamped(a, b, time);
            }
#else
            if (m_UseSlerp)
            {
                return Quaternion.Slerp(a, b, time);
            }
            else
            {
                return Quaternion.Lerp(a, b, time);
            }
#endif
        }

        /// <summary>
        /// Set whether to use slerp for quaternions instead of lerp.
        /// </summary>
        public BXSTweenQuaternionContext SetUseSlerp(bool value)
        {
            m_UseSlerp = value;

            return this;
        }

        /// <summary>
        /// Makes a blank context. Has no duration or anything.
        /// </summary>
        public BXSTweenQuaternionContext()
        { }
        /// <inheritdoc cref="BXSTweenQuaternionContext(float, float, int, EaseType, float)"/>
        public BXSTweenQuaternionContext(float duration)
        {
            SetDuration(duration);
        }
        /// <inheritdoc cref="BXSTweenQuaternionContext(float, float, int, EaseType, float)"/>
        public BXSTweenQuaternionContext(float duration, float delay)
        {
            SetDuration(duration).SetDelay(delay);
        }
        /// <inheritdoc cref="BXSTweenQuaternionContext(float, float, int, EaseType, float)"/>
        public BXSTweenQuaternionContext(float duration, EaseType easing)
        {
            SetDuration(duration).SetEase(easing);
        }
        /// <inheritdoc cref="BXSTweenQuaternionContext(float, float, int, EaseType, float)"/>
        public BXSTweenQuaternionContext(float duration, float delay, int loopCount)
        {
            SetDuration(duration).SetDelay(delay).SetLoopCount(loopCount);
        }
        /// <inheritdoc cref="BXSTweenQuaternionContext(float, float, int, EaseType, float)"/>
        public BXSTweenQuaternionContext(float duration, float delay, int loopCount, float speed)
        {
            SetDuration(duration).SetDelay(delay).SetLoopCount(loopCount).SetSpeed(speed);
        }
        /// <inheritdoc cref="BXSTweenQuaternionContext(float, float, int, EaseType, float)"/>
        public BXSTweenQuaternionContext(float duration, float delay, int loopCount, EaseType easing)
        {
            SetDuration(duration).SetDelay(delay).SetLoopCount(loopCount).SetEase(easing, true);
        }
        /// <summary>
        /// Makes a <see cref="BXSTweenQuaternionContext"/> with predefined settings.
        /// </summary>
        public BXSTweenQuaternionContext(float duration, float delay, int loopCount, EaseType easing, float speed)
        {
            SetDuration(duration).SetDelay(delay).SetLoopCount(loopCount).SetEase(easing, true).SetSpeed(speed);
        }
    }
}
