using System;
using System.Collections.Generic;

namespace BX.Tweening.Interop
{
    /// <summary>
    /// The <see langword="interface"/> that provides the high level game loop.
    /// <br>Each class that require a generic updating method may request any <see cref="IBXSTweenLoop"/>s.</br>
    /// </summary>
    public interface IBXSTweenLoop
    {
        // It is _easier_ to not split this class to:
        // * ITickLoop
        // * IBXSTweenLoop : ITickLoop
        // ---
        // Basically, manage this subsystem seperately from the global tick injection thing, if you happen to have one.

        /// <summary>
        /// An exit action for the tick runner.
        /// </summary>
        /// <param name="applicationQuit">Whether if the application/game is quitting.</param>
        public delegate void ExitAction(IBXSTweenLoop loop, bool applicationQuit);

        // -- Tween Storage
        /// <summary>
        /// The list of running tweens for this loop.
        /// <br>Unless absolutely necessary, there is no need to change the contents of this.</br>
        /// <br>You can use the <see cref="BXSTweenable"/> methods on tweens.</br>
        /// <br/>
        /// <br>It is an erroreneous edge case if a single <see cref="BXSTweenable"/> is shared between two <see cref="IBXSTweenLoop"/>.</br>
        /// </summary>
        public List<BXSTweenable> RunningTweens { get; }
        /// <summary>
        /// The task deferrer; this can be assigned to the default object.
        /// It is used to defer delayed callbacks on the object.
        /// <br>It can only do frame level deferring of callbacks.</br>
        /// </summary>
        public BXSTweenTaskDeferrer<BXSTweenable> TaskDeferrer { get; }
        /// <summary>
        /// The logger value; this can be assigned the <see cref="BXSTweenConsoleLogger"/> if you have nothing.
        /// (or you can implement this interface via proxy class if you don't want or can't have this interface in your own logger)
        /// </summary>
        public IBXSTweenLogger Logger { get; }

        // -- Time
        /// <summary>
        /// The amount of frames or ticks that this runner had.
        /// </summary>
        public int ElapsedTickCount { get; }
        /// <summary>
        /// A unscaled delta time definition.
        /// <br>Return main thread's delta time if unsure.</br>
        /// </summary>
        public float UnscaledDeltaTime { get; }
        /// <summary>
        /// Whether if this runner supports <see cref="OnFixedTick"/>.
        /// </summary>
        public bool SupportsFixedTick { get; }
        /// <summary>
        /// The fixed tick delta time if <see cref="SupportsFixedTick"/> is true.
        /// <br>This value should be ignored if the runner doesn't support fixed tick.</br>
        /// </summary>
        public float FixedUnscaledDeltaTime { get; }
        /// <summary>
        /// The current time scale for this runner.
        /// <br>Return 1f or <c>Time.timeScale</c> (for unity) if unsure.</br>
        /// </summary>
        public float TimeScale { get; }

        // -- Events
        /// <summary>
        /// Called when the BXSTween is created.
        /// </summary>
        public event Action<IBXSTweenLoop> OnInit;
        /// <summary>
        /// A tick method, should be invoked every tick regardless of time scaling.
        /// This method should tick regardless of support and should be ticked usually from the main thread.
        /// <br>Hook into Update/FixedUpdate or '_Process/_PhysicsProcess' if unsure.</br>
        /// </summary>
        public event Action<IBXSTweenLoop> OnTick;
        /// <summary>
        /// A fixed tick method. Called fixed times per second (FixedUpdate)
        /// <br>This should only be used/called if the <see cref="SupportsFixedTick"/> is true.</br>
        /// </summary>
        public event Action<IBXSTweenLoop> OnFixedTick;
        /// <summary>
        /// Should be invoked when the runner is closed/destroyed/disposed.
        /// <br>Hook into OnApplicationQuit/<see cref="Kill"/> if unsure.</br>
        /// </summary>
        public event ExitAction OnExit;

        // -- Management
        /// <summary>
        /// When called should stop/destroy the runner, marking it as non-needed.
        /// <br>This <i>should</i> invoke <see cref="OnExit"/>, if suitable.</br>
        /// </summary>
        public void Kill();
    }
}
