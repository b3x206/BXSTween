using System;
#if UNITY_2017_1_OR_NEWER
#endif
using BX.Tweening.Events;

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
}
