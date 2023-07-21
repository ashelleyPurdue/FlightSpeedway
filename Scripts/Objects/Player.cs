using Godot;
using System;

namespace FlightSpeedway
{
    public partial class Player : CharacterBody3D
    {
        public const float LeftStickDeadzone = 0.1f;

        [Export] public float MaxPitchDegrees = 90;
        [Export] public float MinPitchDegrees = -90;

        [Export] public float PitchRotSpeedDegrees = 90;
        [Export] public float YawRotSpeedDegrees = 90;

        [Export] public float MinFlySpeed = 7;
        [Export] public float MaxFlySpeed = 15;

        [Export] public float MaxAccel = 10;

        [Export] public float ModelRotDecayRate = 5;

        public float PitchRad { get; private set; }
        public float YawRad { get; private set; }
        public float Speed { get; private set; }


        private Node3D _model => GetNode<Node3D>("%Model");

        private float _maxPitchRad => Mathf.DegToRad(MaxPitchDegrees);
        private float _minPitchRad => Mathf.DegToRad(MinPitchDegrees);
        private float _pitchRotSpeedRad => Mathf.DegToRad(PitchRotSpeedDegrees);
        private float _yawRotSpeedRad => Mathf.DegToRad(YawRotSpeedDegrees);


        public override void _PhysicsProcess(double deltaD)
        {
            float delta = (float)deltaD;

            UpdatePitch(delta);
            UpdateYaw(delta);
            UpdateSpeed(delta);

            Vector3 forward = Vector3.Forward
                .Rotated(Vector3.Right, PitchRad)
                .Rotated(Vector3.Up, YawRad);

            Velocity = Speed * forward;

            MoveAndSlide();
        }

        public override void _Process(double deltaD)
        {
            float delta = (float)deltaD;

            _model.Rotation = new Vector3(PitchRad, YawRad, 0);
        }

        private void UpdatePitch(float delta)
        {
            var leftStick = LeftStick();
            PitchRad += leftStick.Y * _pitchRotSpeedRad * delta;
            PitchRad = Mathf.Clamp(PitchRad, _minPitchRad, _maxPitchRad);
        }

        private void UpdateYaw(float delta)
        {
            var leftStick = LeftStick();
            YawRad -= leftStick.X * _yawRotSpeedRad * delta;
            YawRad = Mathf.PosMod(YawRad, Mathf.DegToRad(360));
        }

        private void UpdateSpeed(float delta)
        {
            float t = Mathf.InverseLerp(MinPitchDegrees, MaxPitchDegrees, Mathf.RadToDeg(PitchRad));
            float accel = Mathf.Lerp(MaxAccel, -MaxAccel, t);

            Speed += accel * delta;
            Speed = Mathf.Clamp(Speed, MinFlySpeed, MaxFlySpeed);
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

