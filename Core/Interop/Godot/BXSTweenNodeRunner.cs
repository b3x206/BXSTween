#if GODOT
using Godot;
using System;
using System.Collections.Generic;

namespace BX.Tweening.Interop
{
    /// <summary>
    /// Run BXSTween on a godot node.
    /// </summary>
    public partial class BXSTweenNodeRunner : Node, IBXSTweenLoop
    {
#pragma warning disable IDE0028
#pragma warning disable IDE0090
        private readonly List<BXSTweenable> _RunningTweens = new List<BXSTweenable>();
        public List<BXSTweenable> RunningTweens => _RunningTweens;

        private readonly BXSTweenTaskDeferrer<BXSTweenable> _TaskDeferrer = new BXSTweenTaskDeferrer<BXSTweenable>();
        public BXSTweenTaskDeferrer<BXSTweenable> TaskDeferrer => _TaskDeferrer;

        private readonly BXSTweenGDLogger _Logger = new BXSTweenGDLogger();
        public IBXSTweenLogger Logger => _Logger;

        public int ElapsedTickCount => Engine.GetFramesDrawn();
        public float UnscaledDeltaTime => (float)GetProcessDeltaTime();
        public bool SupportsFixedTick => true;
        public float FixedUnscaledDeltaTime => (float)GetPhysicsProcessDeltaTime();
        public float TimeScale { get; set; } = 1f;

        public event Action<IBXSTweenLoop> OnInit;
        public event Action<IBXSTweenLoop> OnTick;
        public event Action<IBXSTweenLoop> OnFixedTick;
        public event IBXSTweenLoop.ExitAction OnExit;

        public void Kill()
        {
            _RunningTweens.Clear();
            OnExit?.Invoke(this, false);
        }

        public override void _Ready()
        {
            if (BXSTween.NeedsInitialization)
            {
                BXSTween.Initialize(() => this);
            }

            OnInit?.Invoke(this);
        }
        public override void _Process(double delta)
        {
            OnTick?.Invoke(this);
        }
        public override void _PhysicsProcess(double delta)
        {
            OnFixedTick?.Invoke(this);
        }
#pragma warning restore IDE0028
#pragma warning restore IDE0090
    }
}
#endif
