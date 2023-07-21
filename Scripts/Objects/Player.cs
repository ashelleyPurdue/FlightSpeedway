using Godot;
using System;

namespace FlightSpeedway
{
    public partial class Player : CharacterBody3D
    {
        public const float LeftStickDeadzone = 0.1f;

        [Export] public float MaxPitchDegrees = 90;
        [Export] public float MinPitchDegrees = -90;

        [Export] public float PitchRotSpeedDegrees = 180;
        [Export] public float YawRotSpeedDegrees = 180;

        [Export] public float MinFlySpeed = 5;
        [Export] public float MaxFlySpeed = 10;

        [Export] public float MaxAccel = 5;

        [Export] public float ModelRotDecayRate = 5;

        private Node3D _model => GetNode<Node3D>("%Model");

        private float _maxPitchRad => Mathf.DegToRad(MaxPitchDegrees);
        private float _minPitchRad => Mathf.DegToRad(MinPitchDegrees);
        private float _pitchRotSpeedRad => Mathf.DegToRad(PitchRotSpeedDegrees);
        private float _yawRotSpeedRad => Mathf.DegToRad(YawRotSpeedDegrees);

        private float _pitchRad;
        private float _yawRad;
        private float _speed;

        public override void _PhysicsProcess(double deltaD)
        {
            float delta = (float)deltaD;

            UpdatePitch(delta);
            UpdateYaw(delta);
            UpdateSpeed(delta);

            Vector3 forward = Vector3.Forward
                .Rotated(Vector3.Right, _pitchRad)
                .Rotated(Vector3.Up, _yawRad);

            Velocity = _speed * forward;

            MoveAndSlide();
        }

        public override void _Process(double deltaD)
        {
            float delta = (float)deltaD;

            _model.Rotation = new Vector3(_pitchRad, _yawRad, 0);
        }

        private void UpdatePitch(float delta)
        {
            var leftStick = LeftStick();
            _pitchRad += leftStick.Y * _pitchRotSpeedRad * delta;
            _pitchRad = Mathf.Clamp(_pitchRad, _minPitchRad, _maxPitchRad);
        }

        private void UpdateYaw(float delta)
        {
            var leftStick = LeftStick();
            _yawRad += leftStick.X * _yawRotSpeedRad * delta;
            _yawRad = Mathf.PosMod(_yawRad, Mathf.DegToRad(360));
        }

        private void UpdateSpeed(float delta)
        {
            float t = Mathf.InverseLerp(MinPitchDegrees, MaxPitchDegrees, _pitchRad);
            float accel = Mathf.Lerp(MaxAccel, -MaxAccel, t);

            _speed += accel * delta;
            _speed = Mathf.Clamp(_speed, MinFlySpeed, MaxFlySpeed);
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

