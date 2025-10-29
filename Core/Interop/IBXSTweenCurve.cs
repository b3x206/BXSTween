namespace BX.Tweening.Interop
{
    /// <summary>
    /// Interface used to specify custom curve for eases.
    /// </summary>
    public interface IBXSTweenCurve
    {
        /// <summary>
        /// Evaluate the curve value at given time <paramref name="t"/>.
        /// </summary>
        public float Evaluate(float t);
    }
}
