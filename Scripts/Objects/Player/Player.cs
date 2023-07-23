using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace FlightSpeedway
{
    public partial class Player : CharacterBody3D
    {
        [Signal] public delegate void RespawningEventHandler();

        /// <summary>
        ///     Disabling collision shapes infamously can't happen in a
        ///     collision handler, which is inconvenient.
        ///     Here's a shitty workaround.
        /// </summary>
        public bool PretendColliderDisabled {get; set;}

        private PlayerState _currentState;
        private Vector3 _spawnPoint;

        public override void _Ready()
        {
            SignalBus.Instance.LevelReset += Respawn;
            _spawnPoint = Position;

            Respawn();
        }

        public void Respawn()
        {
            EmitSignal(SignalName.Respawning);
            Position = _spawnPoint;
            ChangeState<PlayerFlyState>();
        }

        public void ChangeState<TState>() where TState : PlayerState
        {
            GD.Print($"Changing state to {typeof(TState).Name}");
            _currentState?.OnStateExited();

            foreach (var state in States())
            {
                state.ProcessMode = ProcessModeEnum.Disabled;
            }

            _currentState = States().First(s => s is TState);
            _currentState.ProcessMode = ProcessModeEnum.Inherit;
            _currentState.OnStateEntered();
        }

        private IEnumerable<PlayerState> States()
        {
            for (int i = 0; i < GetChildCount(); i++)
            {
                var child = GetChild<Node>(i);

                if (child is PlayerState state)
                    yield return state;
            }
        }
    }

    public partial class PlayerState : Node
    {
        protected Player _player => GetParent<Player>();
        protected Node3D _model => GetNode<Node3D>("%Model");

        public virtual void OnStateEntered() {}
        public virtual void OnStateExited() {}
    }
}

