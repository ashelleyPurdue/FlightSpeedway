using Godot;

namespace FlightSpeedway
{
    public partial class PlayerFlameDorito : Area3D
    {
        private Node3D _visuals => GetNode<Node3D>("%Visuals");
        private Node3D _model => GetNode<Node3D>("%Model");
        private GpuParticles3D _particles => GetNode<GpuParticles3D>("%Particles");
        private GpuParticles3D _reversedParticles => GetNode<GpuParticles3D>("%ReversedParticles");
        private Light3D _light => GetNode<Light3D>("%Light");
        private CollisionShape3D _shape => GetNode<CollisionShape3D>("%Shape");

        private bool _isActive = false;
        private double _timeRemaining = 0;
        private float _speed;

        private Vector3 _forward => Vector3.Forward
            .Rotated(Vector3.Right, Rotation.X)
            .Rotated(Vector3.Up, Rotation.Y);

        public override void _Ready()
        {
            BodyEntered += OnBodyEntered;
        }

        public void Start(float distance, double time)
        {
            _isActive = true;
            _timeRemaining = time;
            _speed = distance / (float)time;

            _visuals.Position = Vector3.Zero;
            _shape.Position = Vector3.Zero;

            _particles.Lifetime = time;
            var particleMat = (ParticleProcessMaterial)_particles.ProcessMaterial;
            particleMat.InitialVelocityMin = _speed;
            particleMat.InitialVelocityMax = _speed;

            _reversedParticles.Lifetime = time;
            var reversedParticleMat = (ParticleProcessMaterial)_reversedParticles.ProcessMaterial;
            reversedParticleMat.InitialVelocityMin = _speed;
            reversedParticleMat.InitialVelocityMax = _speed;
        }

        public override void _Process(double deltaD)
        {
            float delta = (float)deltaD;
            _visuals.Position += Vector3.Forward * _speed * delta;

            _model.Visible = _isActive;
            _light.Visible = _isActive;

            _particles.Emitting = _isActive;
            _particles.Scale = _isActive
                ? Vector3.One
                : Vector3.One * Mathf.Epsilon;

            _reversedParticles.Emitting = _isActive;
            _reversedParticles.Scale = !_isActive
                ? Vector3.One
                : Vector3.One * Mathf.Epsilon;
        }

        public override void _PhysicsProcess(double deltaD)
        {
            float delta = (float)deltaD;

            _shape.Disabled = !_isActive;
            _shape.Position += Vector3.Forward * _speed * delta;

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