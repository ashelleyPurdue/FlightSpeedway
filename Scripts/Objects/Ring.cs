using System;
using Godot;

namespace FlightSpeedway
{
    public partial class Ring : Area3D
    {
        public bool Alive {get; private set;} = true;

        public override void _Ready()
        {
            SignalBus.Instance.LevelReset += Respawn;
            BodyEntered += OnBodyEntered;
        }

        public void Respawn()
        {
            Alive = true;
        }

        private void OnBodyEntered(Node3D body)
        {
            if (!Alive)
                return;

            if (body is Player)
            {
                Alive = false;
                SignalBus.Instance.EmitRingKilled();
            }
        }
    }
}