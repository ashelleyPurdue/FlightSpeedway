using Godot;
using System;

namespace FlightSpeedway
{
    public partial class Player : CharacterBody3D
    {
        public const float LeftStickDeadzone = 0.1f;

        [Export] public float FlySpeed = 10;
        [Export] public float MaxPitchDegrees = 90;
        [Export] public float MinPitchDegrees = -90;
        [Export] public float TurnRadius = 16;
        [Export] public float PitchReturnToNeutralMult = 0.98f;

        [Export] public float ModelMaxPitch = 45;
        [Export] public float ModelMaxRoll = 45;
        [Export] public float ModelRotDecayRate = 5;

        private Node3D _model => GetNode<Node3D>("%Model");

        public override void _PhysicsProcess(double deltaD)
        {
            float delta = (float)deltaD;
            Vector2 leftStick = LeftStick();

            float rotSpeed = Mathf.RadToDeg(FlySpeed / TurnRadius);

            var rotationDegrees = RotationDegrees;

            if (leftStick.Y != 0)
            {
                rotationDegrees.X += LeftStick().Y * rotSpeed * delta;
                rotationDegrees.X = Mathf.Clamp(rotationDegrees.X, MinPitchDegrees, MaxPitchDegrees);
            }
            else
            {
                rotationDegrees.X *= PitchReturnToNeutralMult;
            }

            rotationDegrees.Y -= LeftStick().X * rotSpeed * delta;

            RotationDegrees = rotationDegrees;
            Velocity = -GlobalTransform.Basis.Z * FlySpeed;
            MoveAndSlide();
        }

        public override void _Process(double deltaD)
        {
            float delta = (float)deltaD;
            var leftStick = LeftStick();

            var targetModelRot = new Vector3(
                Mathf.Lerp(0, ModelMaxPitch, leftStick.Y),
                0,
                Mathf.Lerp(0, ModelMaxRoll, -leftStick.X)
            );

            _model.RotationDegrees = DecayToward(
                _model.RotationDegrees,
                targetModelRot,
                ModelRotDecayRate,
                delta
            );
        }

        private Vector2 LeftStick()
        {
            var raw = new Vector2(
                Input.GetJoyAxis(0, JoyAxis.LeftX),
                Input.GetJoyAxis(0, JoyAxis.LeftY)
            );

            if (raw.Length() < LeftStickDeadzone)
                return Vector2.Zero;

            return raw;
        }

        private Vector3 DecayToward(Vector3 from, Vector3 to, float decayRate, float delta)
        {
            float remaining = from.DistanceTo(to);
            remaining *= Mathf.Pow(Mathf.E, -decayRate * delta);
            return to.MoveToward(from, remaining);
        }
    }
}

