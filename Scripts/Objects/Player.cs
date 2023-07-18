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
        [Export] public float PitchRotSpeed = 180;
        [Export] public float PitchReturnToNeutralSpeed = 45;
        [Export] public float YawRotSpeed = 180;

        public override void _PhysicsProcess(double deltaD)
        {
            float delta = (float)deltaD;
            Vector2 leftStick = LeftStick();

            var rotationDegrees = RotationDegrees;

            if (leftStick.Y != 0)
            {
                rotationDegrees.X += LeftStick().Y * PitchRotSpeed * delta;
                rotationDegrees.X = Mathf.Clamp(rotationDegrees.X, MinPitchDegrees, MaxPitchDegrees);
            }
            else
            {
                rotationDegrees.X = Mathf.MoveToward(rotationDegrees.X, 0, PitchReturnToNeutralSpeed * delta);
            }

            rotationDegrees.Y -= LeftStick().X * YawRotSpeed * delta;

            RotationDegrees = rotationDegrees;
            Velocity = -GlobalTransform.Basis.Z * FlySpeed;
            MoveAndSlide();
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
    }
}

