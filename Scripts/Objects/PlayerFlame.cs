using Godot;

namespace FlightSpeedway
{
    public partial class PlayerFlame : Area3D
    {
        [Export] public CollisionShape3D Collider;
        [Export] public double FlameDuration = 1;
        [Export] public double Cooldown = 1;

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

            _currentState = State.Ready;
            _timer = FlameDuration;
            Collider.Disabled = false;
            Visible = true;
        }

        public override void _Process(double delta)
        {
            switch (_currentState)
            {
                case State.Flaming:
                {
                    _timer -= delta;
                    if (_timer <= 0)
                    {
                        Visible = false;
                        Collider.Disabled = true;
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