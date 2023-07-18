using Godot;
using System;

namespace FlightSpeedway
{
    public partial class Player : CharacterBody3D
    {
        [Export] public float FlySpeed = 10;
        [Export] public float MaxPitchDegrees = 90;
        [Export] public float MinPitchDegrees = -90;
        [Export] public float PitchRotSpeedDegreesPerSecond = 180;
        [Export] public float YawRotSpeedDegreesPerSecond = 180;

        public override void _PhysicsProcess(double deltaD)
        {
            float delta = (float)deltaD;

            var rotationDegrees = RotationDegrees;
            rotationDegrees.X += LeftStick().Y * PitchRotSpeedDegreesPerSecond * delta;
            rotationDegrees.Y -= LeftStick().X * YawRotSpeedDegreesPerSecond * delta;

            rotationDegrees.X = Mathf.Clamp(rotationDegrees.X, MinPitchDegrees, MaxPitchDegrees);

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

            if (raw.Length() < 0.1f)
                return Vector2.Zero;

            return raw;
        }
    }
}

