using Godot;

namespace FlightSpeedway
{
    public partial class PlayerFlame : Area3D
    {
        [Export] public double FlameDuration = 0.4;
        [Export] public double Cooldown = 0.4;

        private CollisionShape3D _collider => GetNode<CollisionShape3D>("%FlameCollisionShape");
        private GpuParticles3D _particles => GetNode<GpuParticles3D>("%FlameParticles");
        private Light3D _light => GetNode<Light3D>("%FlameLight");

        private double _timer;

        private float _lightTargetEnergy = 0;
        private float _lightEnergyChangeSpeed => 1f / ((float)FlameDuration / 2);

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

            _collider.Disabled = _currentState != State.Flaming;
        }

        public override void _Process(double delta)
        {
            _particles.Emitting = _currentState == State.Flaming;
            _lightTargetEnergy = _currentState == State.Flaming
                ? 1
                : 0;

            _light.LightEnergy = Mathf.MoveToward(
                _light.LightEnergy,
                _lightTargetEnergy,
                _lightEnergyChangeSpeed * (float)delta
            );
        }
    }
}