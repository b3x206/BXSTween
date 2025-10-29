namespace BX.Tweening.Interop
{
    /// <summary>
    /// The targeted ticking type.
    /// <br><see cref="Variable"/> : The tweening will be called on a method like 'Update' or '_process', where it is called every frame.</br>
    /// <br><see cref="Fixed"/> : The tweening will be called on a method like 'FixedUpdate' or '_physics_process', where it is called every n times per second.</br>
    /// </summary>
    public enum TickType
    {
        Variable,
        Fixed
    }
}
