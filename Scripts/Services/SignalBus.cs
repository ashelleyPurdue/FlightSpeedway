using Godot;

namespace FlightSpeedway
{
    public partial class SignalBus : Node
    {
        public static SignalBus Instance {get; private set;}

        [Signal] public delegate void LevelResetEventHandler();
        [Signal] public delegate void RingKilledEventHandler();

        public override void _Ready()
        {
            Instance = this;
        }

        public void EmitRingKilled() => EmitSignal(SignalName.RingKilled);
    }
}