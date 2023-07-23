using Godot;

namespace FlightSpeedway
{
    public partial class PlayerWaterCrashState : PlayerState
    {
        [Export] public double Duration = 2;
        [Export] public float HSpeedDecayRate = 5;
        [Export] public float VerticalSpringConstant = 20;
        [Export] public float VerticalSpringDamping = 0.95f;

        [Export] public float RotationRestoreDecayRate = 0.95f;

        private double _timer = 0;
        private float _targetY;

        public override void OnStateEntered()
        {
            _timer = Duration;
            _targetY = _player.Position.Y;

            var rot = _player.RotationDegrees;
            rot.X = 0;
            _player.RotationDegrees = rot;
        }

        public override void _PhysicsProcess(double deltaD)
        {
            float delta = (float)deltaD;

            _timer -= deltaD;
            if (_timer <= 0)
            {
                SignalBus.Instance.EmitLevelReset();
                return;
            }

            UpdateHSpeed(delta);
            UpdateVSpeed(delta);

            _player.MoveAndSlide();
        }

        public override void _Process(double deltaD)
        {
            float delta = (float)deltaD;

            _model.RotationDegrees = MathUtils.DecayToward(
                _model.RotationDegrees,
                Vector3.Zero,
                RotationRestoreDecayRate,
                delta
            );
        }

        private void UpdateHSpeed(float delta)
        {
            Vector3 flatVel = _player.Velocity;
            flatVel.Y = 0;
            flatVel = MathUtils.DecayToward(
                flatVel,
                Vector3.Zero,
                HSpeedDecayRate,
                delta
            );

            _player.Velocity = flatVel + (Vector3.Up * _player.Velocity.Y);
        }

        private void UpdateVSpeed(float delta)
        {
            var velocity = _player.Velocity;

            velocity.Y += (_targetY - _player.Position.Y) * VerticalSpringConstant * delta;
            velocity.Y = MathUtils.DecayToward(
                velocity.Y,
                0,
                VerticalSpringDamping,
                delta
            );

            _player.Velocity = velocity;
        }
    }
}