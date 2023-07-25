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

            _collider.Disabled = false;
            _particles.Emitting = true;
            _lightTargetEnergy = 1;
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
                        _currentState = State.CoolingDown;
                        _timer = Cooldown;

                        _collider.Disabled = true;
                        _particles.Emitting = false;
                        _lightTargetEnergy = 0;
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

            _light.LightEnergy = Mathf.MoveToward(
                _light.LightEnergy,
                _lightTargetEnergy,
                _lightEnergyChangeSpeed * (float)delta
            );
        }
    }
}