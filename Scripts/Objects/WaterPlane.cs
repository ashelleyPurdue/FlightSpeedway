using Godot;

namespace FlightSpeedway
{
    public partial class WaterPlane : Area3D
    {
        public override void _Ready()
        {
            BodyEntered += OnBodyEntered;
        }

        private void OnBodyEntered(Node3D body)
        {
            if (body is Player)
                SignalBus.Instance.EmitLevelReset();
        }
    }
}