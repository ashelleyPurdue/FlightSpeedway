using Godot;

namespace FlightSpeedway
{
    public partial class PlayerFlame : Node3D
    {
        [Export] public double FlameDuration = 0.4;
        [Export] public double Cooldown = 0.4;
        [Export] public float FlameDistance = 3;
        [Export] public float FlameAngleDeg = 45;
        [Export] public int Rings = 2;
        [Export] public int DoritosPerRing = 6;
        [Export] public PackedScene DoritoPrefab;

        private double _cooldownTimer = 0;

        public override void _Ready()
        {
            for (int i = 0; i < Rings; i++)
            {
                float ringAngleDeg = (i + 1) * FlameAngleDeg / Rings;
                CreateRing(ringAngleDeg, DoritosPerRing, i);
            }

            // Put one more dorito in the very center
            CreateDorito(0, 0);

            void CreateRing(
                float ringAngleDeg,
                int doritos,
                int ringIndex
            )
            {
                float degreesPerDorito = 360f / doritos;
                float doritoAngleOffset = ringIndex % 2 == 0
                    ? 0
                    : degreesPerDorito / 2;

                for (int i = 0; i < doritos; i++)
                {
                    float doritoAngleDeg = i * degreesPerDorito;
                    doritoAngleDeg += doritoAngleOffset;
                    CreateDorito(ringAngleDeg, doritoAngleDeg);
                }
            }

            void CreateDorito(float ringAngleDeg, float doritoAngleDeg)
            {
                var dorito = DoritoPrefab.Instantiate<PlayerFlameDorito>();
                AddChild(dorito);

                float ringAngleRad = Mathf.DegToRad(ringAngleDeg);
                float doritoAngleRad = Mathf.DegToRad(doritoAngleDeg);

                RotateAboutAxis(dorito, Vector3.Up, ringAngleRad);
                RotateAboutAxis(dorito, Vector3.Forward, doritoAngleRad);
            }

            void RotateAboutAxis(Node3D node, Vector3 axis, float angleRad)
            {
                // TODO: Find a way to do this without creating a temporary
                // holder node.
                var oldParent = node.GetParent();
                var holder = new Node3D();

                oldParent.AddChild(holder);
                oldParent.RemoveChild(node);
                holder.AddChild(node);

                holder.Rotate(axis, angleRad);
                var globalRot = node.GlobalRotation;

                holder.RemoveChild(node);
                holder.QueueFree();
                oldParent.AddChild(node);
                node.GlobalRotation = globalRot;
            }
        }

        public void Flame()
        {
            if (_cooldownTimer > 0)
                return;

            _cooldownTimer = FlameDuration + Cooldown;

            foreach (var child in GetChildren())
            {
                var dorito = (PlayerFlameDorito)child;
                dorito.Start(FlameDistance, FlameDuration);
            }
        }

        public override void _PhysicsProcess(double delta)
        {
            if (_cooldownTimer > 0)
                _cooldownTimer -= delta;
        }
    }
}