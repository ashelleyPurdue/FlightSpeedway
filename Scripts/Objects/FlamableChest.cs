using System;
using Godot;

namespace FlightSpeedway
{
    public partial class FlamableChest : StaticBody3D, IFlamable
    {
        public bool Alive {get; private set;} = true;

        private CollisionShape3D _collider => GetNode<CollisionShape3D>("%CollisionShape");

        public override void _Ready()
        {
            SignalBus.Instance.LevelReset += Respawn;
        }

        public override void _Process(double delta)
        {
            Visible = Alive;
        }

        public void Respawn()
        {
            Alive = true;
            _collider.SetDeferred("disabled", false);
        }

        public void OnFlamed()
        {
            if (!Alive)
                return;

            Alive = false;
            _collider.SetDeferred("disabled", true);
        }
    }

    public interface IFlamable
    {
        void OnFlamed();
    }
}