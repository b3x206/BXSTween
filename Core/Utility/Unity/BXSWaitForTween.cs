#if UNITY_5_6_OR_NEWER
using System;
using UnityEngine;

namespace BX.Tweening
{
    /// <summary>
    /// Waits a tween until it's finished.
    /// <br>In case the tween throws an exception, this will stop waiting.</br>
    /// <br>If a tween is paused it won't wait unless <see cref="waitWhilePaused"/> is true.</br>
    /// </summary>
    public sealed class BXSWaitForTween : CustomYieldInstruction
    {
        public override bool keepWaiting => tweenable.IsPlaying || (waitWhilePaused && tweenable.IsPaused) || tweenable.Owner.TaskDeferrer.IsScheduled(tweenable);

        private readonly bool waitWhilePaused = false;
        private readonly BXSTweenable tweenable;
        public BXSWaitForTween(BXSTweenable target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target), "[BXSWaitForTween::.ctor] Given BXSTweenable target cannot be null.");
            }

            tweenable = target;
        }
        public BXSWaitForTween(BXSTweenable target, bool waitWhilePaused) : this(target)
        {
            this.waitWhilePaused = waitWhilePaused;
        }
    }
}
#endif
