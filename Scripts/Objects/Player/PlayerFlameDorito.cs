using Godot;

namespace FlightSpeedway
{
    public partial class PlayerFlameDorito : Area3D
    {
        private Node3D _model => GetNode<Node3D>("%Model");
        private CollisionShape3D _shape => GetNode<CollisionShape3D>("%Shape");
        private GpuParticles3D _particles => GetNode<GpuParticles3D>("%Particles");
        private Light3D _light => GetNode<Light3D>("%Light");

        private bool _isActive = false;
        private double _timeRemaining = 0;
        private float _speed;

        public override void _Ready()
        {
            BodyEntered += OnBodyEntered;
        }

        public void Start(float distance, double time)
        {
            _isActive = true;
            _timeRemaining = time;
            _speed = distance / (float)time;

            Position = Vector3.Zero;

            _particles.Lifetime = time;
            var particleMat = (ParticleProcessMaterial)_particles.ProcessMaterial;
            particleMat.InitialVelocityMin = _speed;
            particleMat.InitialVelocityMax = _speed;
        }

        public override void _PhysicsProcess(double deltaD)
        {
            float delta = (float)deltaD;

            _model.Visible = _isActive;
            _particles.Emitting = _isActive;
            _light.Visible = _isActive;
            _shape.Disabled = !_isActive;

            Vector3 forward = Vector3.Forward
                .Rotated(Vector3.Right, Rotation.X)
                .Rotated(Vector3.Up, Rotation.Y);

            Position += forward * _speed * delta;

            if (_isActive)
            {
                _timeRemaining -= delta;

                if (_timeRemaining <= 0)
                    _isActive = false;
            }
        }

        private void OnBodyEntered(Node3D body)
        {
            _isActive = false;

            if (body is IFlamable f)
                f.OnFlamed();
        }
    }
}