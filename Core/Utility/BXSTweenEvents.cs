namespace BX.Tweening.Events
{
    /// <summary>
    /// A blank void action.
    /// </summary>
    public delegate void BXSAction();
    /// <summary>
    /// A void action that takes a generic parameter with value.
    /// </summary>
    public delegate void BXSAction<in T>(T value);

    /// <summary>
    /// A boolean action that returns a condition.
    /// </summary>
    public delegate bool BXSConditionAction();
    /// <summary>
    /// A tick condition return action.
    /// </summary>
    public delegate TickSuspendAction BXSTickConditionAction();
    /// <summary>
    /// An equality comparison handler.
    /// </summary>
    public delegate bool BXSEqualityComparison<in T>(T lhs, T rhs);

    /// <summary>
    /// An action called when the <see cref="IBXSTweenLoop"/> is about to quit.
    /// </summary>
    /// <param name="cleanup">Whether if the tween runner was closed for good.</param>
    public delegate void BXSExitAction(bool cleanup);

    /// <summary>
    /// An action called to get a value out.
    /// </summary>
    public delegate T BXSGetterAction<out T>();
    /// <summary>
    /// An action called to get a value out.
    /// </summary>
    public delegate T BXSGetterWithRefAction<out T, in TObject>(TObject target);
    /// <summary>
    /// An action called to set a value on given delegate.
    /// </summary>
    public delegate void BXSSetterAction<in T>(T value);
    /// <summary>
    /// An action called to set a value on given delegate.
    /// </summary>
    public delegate void BXSSetterWithRefAction<in T, in TObject>(TObject target, T value);

    /// <summary>
    /// An ease action.
    /// <br>Expected to return from-to 0-1, but can overshoot.</br>
    /// </summary>
    /// <param name="time">Time for this tween. This parameter is linearly interpolated.</param>
    /// <returns>The eased value.</returns>
    public delegate float BXSEaseAction(float time);
}
