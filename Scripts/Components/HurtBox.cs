using System;
using Godot;

namespace FlightSpeedway
{
    public partial class HurtBox : Area3D
    {
        [Signal] public delegate void TookDamageEventHandler();

        public override void _Ready()
        {
            AreaEntered += OnAreaEntered;
        }

        private void OnAreaEntered(Area3D other)
        {
            if (other is PlayerFlame)
                EmitSignal(SignalName.TookDamage);
        }
    }
}