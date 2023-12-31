using Godot;
using System;

namespace FlightSpeedway
{
    public partial class PlayerFlyState : PlayerState
    {
        [Export] public float MaxPitchDegrees = 90;
        [Export] public float MinPitchDegrees = -90;

        [Export] public float PitchRotSpeedDegrees = 90;
        [Export] public float PitchRotAccelDegrees = 360;
        [Export] public float YawRotSpeedDegrees = 90;
        [Export] public float YawRotAccelDegrees = 360;

        [Export] public float MinFlySpeed = 15;
        [Export] public float MaxFlySpeed = 20;

        [Export] public float MaxAccel = 10;

        [Export] public float ModelMaxPitchDegrees = 45;
        [Export] public float ModelMaxRollDegrees = 45;
        [Export] public float ModelRotDecayRate = 5;

        public float PitchRad { get; private set; }
        public float YawRad { get; private set; }
        public float Speed { get; private set; }

        private float _maxPitchRad => Mathf.DegToRad(MaxPitchDegrees);
        private float _minPitchRad => Mathf.DegToRad(MinPitchDegrees);
        private float _maxPitchRotSpeedRad => Mathf.DegToRad(PitchRotSpeedDegrees);
        private float _pitchRotAccelRad => Mathf.DegToRad(PitchRotAccelDegrees);
        private float _maxYawRotSpeedRad => Mathf.DegToRad(YawRotSpeedDegrees);
        private float _yawRotAccelRad => Mathf.DegToRad(YawRotAccelDegrees);

        private float _pitchRotSpeedRad;
        private float _yawRotSpeedRad;

        private PlayerFlame _flame => GetNode<PlayerFlame>("%Flame");

        public override void _Ready()
        {
            _player.Respawning += OnRespawning;
        }

        public void OnRespawning()
        {
            PitchRad = 0;
            YawRad = 0;
            Speed = MinFlySpeed;

            _pitchRotSpeedRad = 0;
            _yawRotSpeedRad = 0;
        }

        public override void _Input(InputEvent ev)
        {
            if (InputService.FlameJustPressed(ev))
                _flame.Flame();
        }

        public override void _PhysicsProcess(double deltaD)
        {
            float delta = (float)deltaD;

            UpdatePitch(delta);
            UpdateYaw(delta);
            UpdateSpeed(delta);

            Vector3 forward = Vector3.Forward
                .Rotated(Vector3.Right, PitchRad)
                .Rotated(Vector3.Up, YawRad);

            _player.Velocity = Speed * forward;
            _player.Rotation = new Vector3(PitchRad, YawRad, 0);

            _player.MoveAndSlide();
        }

        public override void _Process(double deltaD)
        {
            float delta = (float)deltaD;

            var targetModelRot = new Vector3(
                Mathf.Lerp(0, ModelMaxPitchDegrees, -LeftStick().Y),
                Mathf.Lerp(0, ModelMaxRollDegrees, -LeftStick().X),
                Mathf.Lerp(0, ModelMaxRollDegrees, -LeftStick().X)
            );

            _model.RotationDegrees = MathUtils.DecayToward(
                _model.RotationDegrees,
                targetModelRot,
                ModelRotDecayRate,
                delta
            );
        }

        private void UpdatePitch(float delta)
        {
            float targetPitchRotSpeedRad = _maxPitchRad * -LeftStick().Y;
            _pitchRotSpeedRad = Mathf.MoveToward(
                _pitchRotSpeedRad,
                targetPitchRotSpeedRad,
                _pitchRotAccelRad * delta
            );

            PitchRad += _pitchRotSpeedRad * delta;
            PitchRad = Mathf.Clamp(PitchRad, _minPitchRad, _maxPitchRad);
        }

        private void UpdateYaw(float delta)
        {
            float targetYawRotSpeedRad = -LeftStick().X * _maxYawRotSpeedRad;
            _yawRotSpeedRad = Mathf.MoveToward(
                _yawRotSpeedRad,
                targetYawRotSpeedRad,
                _yawRotAccelRad * delta
            );

            YawRad += _yawRotSpeedRad * delta;
            YawRad = Mathf.PosMod(YawRad, Mathf.DegToRad(360));
        }

        private void UpdateSpeed(float delta)
        {
            float t = Mathf.InverseLerp(MinPitchDegrees, MaxPitchDegrees, Mathf.RadToDeg(PitchRad));
            float accel = Mathf.Lerp(MaxAccel, -MaxAccel, t);

            Speed += accel * delta;
            Speed = Mathf.Clamp(Speed, MinFlySpeed, MaxFlySpeed);
        }

        private static Vector2 LeftStick() => InputService.LeftStick;
    }
}

