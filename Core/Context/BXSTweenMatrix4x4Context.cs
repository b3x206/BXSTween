using System;
#if UNITY_5_6_OR_NEWER
using UnityEngine;
#else
using System.Numerics;
#endif

namespace BX.Tweening
{
    /// <summary>
    /// Contains a context that uses Matrix4x4.
    /// </summary>
    [Serializable]
    public sealed class BXSTweenMatrix4x4Context : BXSTweenContext<Matrix4x4>
    {
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static float Lerp(float a, float b, float time)
        {
            return ((1f - time) * a) + (time * b);
        }

        public override Matrix4x4 Lerp(Matrix4x4 a, Matrix4x4 b, float time)
        {
            Matrix4x4 result = default;

#if UNITY_5_6_OR_NEWER
            // eh, the compiler will unroll this if it wants it.
            for (int i = 0; i < 16; i++)
            {
                result[i] = Lerp(a[i], b[i], time);
            }
#else
            result = Matrix4x4.Lerp(a, b, time);
#endif
            return result;
        }

        /// <summary>
        /// Makes a blank context. Has no duration or anything.
        /// </summary>
        public BXSTweenMatrix4x4Context()
        { }
        /// <inheritdoc cref="BXSTweenMatrix4x4Context(float, float, int, EaseType, float)"/>
        public BXSTweenMatrix4x4Context(float duration)
        {
            SetDuration(duration);
        }
        /// <inheritdoc cref="BXSTweenMatrix4x4Context(float, float, int, EaseType, float)"/>
        public BXSTweenMatrix4x4Context(float duration, float delay)
        {
            SetDuration(duration).SetDelay(delay);
        }
        /// <inheritdoc cref="BXSTweenMatrix4x4Context(float, float, int, EaseType, float)"/>
        public BXSTweenMatrix4x4Context(float duration, EaseType easing)
        {
            SetDuration(duration).SetEase(easing);
        }
        /// <inheritdoc cref="BXSTweenMatrix4x4Context(float, float, int, EaseType, float)"/>
        public BXSTweenMatrix4x4Context(float duration, float delay, int loopCount)
        {
            SetDuration(duration).SetDelay(delay).SetLoopCount(loopCount);
        }
        /// <inheritdoc cref="BXSTweenMatrix4x4Context(float, float, int, EaseType, float)"/>
        public BXSTweenMatrix4x4Context(float duration, float delay, int loopCount, float speed)
        {
            SetDuration(duration).SetDelay(delay).SetLoopCount(loopCount).SetSpeed(speed);
        }
        /// <inheritdoc cref="BXSTweenMatrix4x4Context(float, float, int, EaseType, float)"/>
        public BXSTweenMatrix4x4Context(float duration, float delay, int loopCount, EaseType easing)
        {
            SetDuration(duration).SetDelay(delay).SetLoopCount(loopCount).SetEase(easing, true);
        }
        /// <summary>
        /// Makes a <see cref="BXSTweenMatrix4x4Context"/> with predefined settings.
        /// </summary>
        public BXSTweenMatrix4x4Context(float duration, float delay, int loopCount, EaseType easing, float speed)
        {
            SetDuration(duration).SetDelay(delay).SetLoopCount(loopCount).SetEase(easing, true).SetSpeed(speed);
        }
    }
}
