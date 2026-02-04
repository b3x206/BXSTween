using BX.Tweening.Events;
using System;
using System.Runtime.CompilerServices;

namespace BX.Tweening
{
    /// <summary>
    /// Easing type of the tweenings, the easing functions are gathered from the <see cref="BXSTweenEase"/>.
    /// <br>
    /// You can see an approximation curve in any <see cref="EaseType"/> property selector or look at the website https://easings.net/
    /// </br>
    /// </summary>
    public enum EaseType
    {
        Linear,
        QuadIn,
        QuadOut,
        QuadInOut,
        CubicIn,
        CubicOut,
        CubicInOut,
        QuartIn,
        QuartOut,
        QuartInOut,
        QuintIn,
        QuintOut,
        QuintInOut,
        BounceIn,
        BounceOut,
        BounceInOut,
        ElasticIn,
        ElasticOut,
        ElasticInOut,
        CircularIn,
        CircularOut,
        CircularInOut,
        SinusIn,
        SinusOut,
        SinusInOut,
        ExponentialIn,
        ExponentialOut,
        ExponentialInOut
    }

    /// <summary>
    /// Includes the hard coded ease types.
    /// To create custom ease curves you can use the <see cref="AnimationCurve"/>.
    /// </summary>
    public static class BXSTweenEase
    {
        // !! DON'T CHANGE THE ORDER UNLESS YOU ARE ADDING EASE !! //
        //    (this code is indeed an hotpath, so this will do)    //
        /// <summary>
        /// Lookup for ease functions.
        /// </summary>
        private static readonly BXSEaseAction[] EaseFunctions =
        {
            Linear,
            QuadIn,
            QuadOut,
            QuadInOut,
            CubicIn,
            CubicOut,
            CubicInOut,
            QuartIn,
            QuartOut,
            QuartInOut,
            QuintIn,
            QuintOut,
            QuintInOut,
            BounceIn,
            BounceOut,
            BounceInOut,
            ElasticIn,
            ElasticOut,
            ElasticInOut,
            CircularIn,
            CircularOut,
            CircularInOut,
            SinusIn,
            SinusOut,
            SinusInOut,
            ExponentialIn,
            ExponentialOut,
            ExponentialInOut,
        };

        public static int EaseFunctionsCount => EaseFunctions.Length;
        /// <summary>
        /// Get an ease function at index.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BXSEaseAction GetEaseFunction(int index)
        {
            return EaseFunctions[index];
        }
        /// <summary>
        /// Get an ease function matching <paramref name="easing"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BXSEaseAction GetEaseFunction(EaseType easing)
        {
            return EaseFunctions[(int)easing];
        }

        /// The inline version for getting the eased value, without any delegate stuff.
        /// Because the delegate stuff allocates garbage and i have enough garbage in my pc such as my code lol.
        /// <summary>
        /// Returns a eased in value.
        /// <br>The returned value is not clamped between 0-1.</br>
        /// </summary>
        /// <param name="time">The time parameter. This is expected to be between 0-1 but it's not enforced.</param>
        /// <param name="easing">
        /// The corresponding easing to get for this type.
        /// By easing out the time parameter of a unclamped lerp you can get differently animated values.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EasedValue(float time, EaseType easing)
        {
            return EaseFunctions[(int)easing](time);
        }

        #region Ease Methods
        // Note : All ease methods are unclamped.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float Linear(float t)
        {
            float tVal = t;
            return tVal;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float QuadIn(float t)
        {
            float tVal = t * t;
            return tVal;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float QuadOut(float t)
        {
            float tVal = t * (2f - t);
            return tVal;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float QuadInOut(float t)
        {
            float tVal = t < 0.5f ? 2f * t * t : -1f + ((4f - (2f * t)) * t);
            return tVal;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float CubicIn(float t)
        {
            float tVal = t * t * t;
            return tVal;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float CubicOut(float t)
        {
            float tSub = 1f - t;
            float tVal = 1f - (tSub * tSub * tSub);
            return tVal;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float CubicInOut(float t)
        {
            float tVal = t < 0.5f ? 4f * t * t * t : ((t - 1f) * ((2f * t) - 2f) * ((2 * t) - 2)) + 1f;
            return tVal;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float QuartIn(float t)
        {
            float tVal = t * t * t * t;
            return tVal;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float QuartOut(float t)
        {
            float tInv = 1f - t; // inverted t (assuming t = clamped between 0-1)
            float tVal = 1f - (tInv * tInv * tInv * tInv);
            return tVal;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float QuartInOut(float t)
        {
            float tI2v = (-2 * t) + 2; // (-2 * x) + 2
            float tVal = t < 0.5f ? 8f * t * t * t * t : 1f - ((tI2v * tI2v * tI2v * tI2v) / 2f);
            return tVal;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float QuintIn(float t)
        {
            float tVal = t * t * t * t * t;
            return tVal;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float QuintOut(float t)
        {
            float tInv = 1f - t; // inverted t (assuming t = clamped between 0-1)
            float tVal = 1f - (tInv * tInv * tInv * tInv * tInv);
            return tVal;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float QuintInOut(float t)
        {
            float tI2v = (-2 * t) + 2; // (-2 * x) + 2
            float tVal = t < 0.5f ? 16f * t * t * t * t * t : 1f - ((tI2v * tI2v * tI2v * tI2v * tI2v) / 2f);
            return tVal;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float BounceIn(float t)
        {
            float tVal = 1f - BounceOut(1f - t);
            return tVal;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float BounceOut(float t)
        {
            float tVal = t < 0.363636374f ? 7.5625f * t * t : t < 0.727272749f ? (7.5625f * (t -= 0.545454562f) * t) + 0.75f : t < 0.909090936f ? (7.5625f * (t -= 0.8181818f) * t) + 0.9375f : (7.5625f * (t -= 21f / 22f) * t) + (63f / 64f);
            return tVal;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float BounceInOut(float t)
        {
            float tVal = t < 0.5f ? BounceIn(t * 2f) * 0.5f : (BounceOut((t * 2f) - 1f) * 0.5f) + 0.5f;
            return tVal;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float ElasticIn(float t)
        {
            float tVal = -(MathF.Pow(2, 10 * (t -= 1)) * MathF.Sin((t - (0.3f / 4f)) * (2 * MathF.PI) / 0.3f));
            return tVal;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float ElasticOut(float t)
        {
            float tVal = t == 1f ? 1f : 1f - ElasticIn(1f - t);
            return tVal;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float ElasticInOut(float t)
        {
            float tVal = (t *= 2f) == 2f ? 1f : t < 1f ? -0.5f * (MathF.Pow(2f, 10f * (t -= 1)) * MathF.Sin((t - 0.1125f) * (2f * MathF.PI) / 0.45f)) :
                ((MathF.Pow(2f, -10f * (t -= 1f)) * MathF.Sin((t - 0.1125f) * (2f * MathF.PI) / 0.45f) * 0.5f) + 1f);
            return tVal;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float CircularIn(float t)
        {
            float tVal = -(MathF.Sqrt(1 - (t * t)) - 1);
            return tVal;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float CircularOut(float t)
        {
            float tVal = MathF.Sqrt(1f - ((t -= 1f) * t));
            return tVal;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float CircularInOut(float t)
        {
            float tVal = (t *= 2f) < 1f ? -1f / 2f * (MathF.Sqrt(1f - (t * t)) - 1f) : 0.5f * (MathF.Sqrt(1 - ((t -= 2) * t)) + 1);
            return tVal;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float SinusIn(float t)
        {
            float tVal = -MathF.Cos(t * (MathF.PI * 0.5f)) + 1f;
            return tVal;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float SinusOut(float t)
        {
            float tVal = MathF.Sin(t * (MathF.PI * 0.5f));
            return tVal;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float SinusInOut(float t)
        {
            float tVal = -0.5f * (MathF.Cos(MathF.PI * t) - 1f);
            return tVal;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float ExponentialIn(float t)
        {
            float tVal = MathF.Pow(2f, 10f * (t - 1f));
            return tVal;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float ExponentialOut(float t)
        {
            float tVal = MathF.Sin(t * (MathF.PI * 0.5f));
            return tVal;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float ExponentialInOut(float t)
        {
            float tVal = -0.5f * (MathF.Cos(MathF.PI * t) - 1f);
            return tVal;
        }
        #endregion
    }
}
