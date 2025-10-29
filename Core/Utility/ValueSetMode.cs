namespace BX.Tweening
{
    /// <summary>
    /// Used to specify set mode for values that can be added, equated or removed from.
    /// </summary>
    public enum ValueSetMode
    {
        /// <summary>
        /// Sets value with <c>=</c>
        /// </summary>
        Equals,
        /// <summary>
        /// Removes from value with <c>-=</c>
        /// <br>For enumerations with [<see cref="System.FlagsAttribute"/>], this is <c>&amp;= ~<see langword="value"/></c></br>
        /// </summary>
        Remove,
        /// <summary>
        /// Adds to value with <c>+=</c>
        /// <br>For enumerations with [<see cref="System.FlagsAttribute"/>], this is <c>|= <see langword="value"/></c></br>
        /// </summary>
        Add,
    }
}
