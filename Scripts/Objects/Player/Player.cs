using System;
using System.Collections.Generic;
using Godot;

namespace FlightSpeedway
{
    public partial class Player : CharacterBody3D
    {
        [Signal] public delegate void RespawningEventHandler();

        public override void _Ready()
        {
            SignalBus.Instance.LevelReset += Respawn;
            Respawn();
        }

        public void Respawn()
        {
            EmitSignal(SignalName.Respawning);
            ChangeState<PlayerFlyState>();
        }

        public void ChangeState<TState>() where TState : PlayerState
        {
            foreach (var state in States())
            {
                state.ProcessMode = state is TState
                    ? ProcessModeEnum.Inherit
                    : ProcessModeEnum.Disabled;
            }
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

    public partial class PlayerState : Node {}
}

