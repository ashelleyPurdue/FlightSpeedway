using Godot;

namespace FlightSpeedway
{
    public partial class PlayerFlame : Node3D
    {
        [Export] public double FlameDuration = 0.4;
        [Export] public double Cooldown = 0.4;
        [Export] public float FlameDistance = 3;
        [Export] public float FlameAngleDeg = 45f;
        [Export] public int Rows = 3;
        [Export] public int DoritosPerRow = 3;
        [Export] public PackedScene DoritoPrefab;

        private double _timer;

        private enum State
        {
            Ready,
            Flaming,
            CoolingDown
        }
        private State _currentState = State.Ready;

        public override void _Ready()
        {
            float degreesPerRow = FlameAngleDeg / Rows;
            float degreesPerDorito = FlameAngleDeg / DoritosPerRow;

            for (int r = 0; r < Rows; r++)
            {
                for (int d = 0; d < DoritosPerRow; d++)
                {
                    var dorito = DoritoPrefab.Instantiate<PlayerFlameDorito>();
                    dorito.RotationDegrees = new Vector3(
                        degreesPerRow * r - (FlameAngleDeg / 2),
                        degreesPerDorito * d - (FlameAngleDeg / 2),
                        0
                    );

                    AddChild(dorito);
                }
            }
        }

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