
namespace BX.Tweening
{
    /// <summary>
    /// This interface can be used in the supplied link object of <see cref="BXSTweenContext{TValue}"/> to check validity.
    /// <br>It is checked every tick, not by <see cref="BXSTween.LinkObjectComparison"/> but by type testing.</br>
    /// </summary>
    public interface IBXSTweenLinkObject
    {
        /// <summary>
        /// Whether to only check the validity of this object once in the simulation loop.
        /// <br>Since the simulation loop drains the deltatime the validity check may occur more than once in the loop.</br>
        /// </summary>
        public bool CheckValidityOnce { get; }

        /// <summary>
        /// Called every tick on the specified link object if it's attached to a tween.
        /// <br>The results can be cached per tick if <see cref="CheckValidityOnce"/> is <see langword="true"/>.</br>
        /// </summary>
        /// <returns> <see langword="true"/> if this object/handle is valid, <see langword="false"/> otherwise. </returns>
        public bool IsValid();
    }
}
