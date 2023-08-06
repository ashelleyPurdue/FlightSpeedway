using Godot;

namespace FlightSpeedway
{
    public partial class PlayerFlame : Node3D
    {
        [Export] public double FlameDuration = 0.4;
        [Export] public double Cooldown = 0.4;
        [Export] public float FlameDistance = 5;

        private double _timer;

        private enum State
        {
            Ready,
            Flaming,
            CoolingDown
        }
        private State _currentState = State.Ready;

        public void Flame()
        {
            if (_currentState != State.Ready)
                return;

            _currentState = State.Flaming;
            _timer = FlameDuration;

            foreach (var child in GetChildren())
            {
                var dorito = (PlayerFlameDorito)child;
                dorito.Start(FlameDistance, FlameDuration);
            }
        }

        public override void _PhysicsProcess(double delta)
        {
            switch (_currentState)
            {
                case State.Flaming:
                {
                    _timer -= delta;
                    if (_timer <= 0)
                    {
                        _currentState = State.CoolingDown;
                        _timer = Cooldown;
                    }
                    break;
                }

                case State.CoolingDown:
                {
                    _timer -= delta;
                    if (_timer <= 0)
                    {
                        _currentState = State.Ready;
                    }
                    break;
                }
            }
        }
    }
}