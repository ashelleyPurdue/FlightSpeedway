using System;
using Godot;

namespace FlightSpeedway
{
    [Tool]
    [GlobalClass]
    public partial class RingPath : Path3D
    {
        [Export] public PackedScene RingPrefab
        {
            get => _ringPrefab;
            set
            {
                _ringPrefab = value;
                Refresh();
            }
        }

        [Export] public int RingCount
        {
            get => _ringCount;
            set
            {
                _ringCount = value;
                Refresh();
            }
        }

        private PackedScene _ringPrefab;
        private int _ringCount;


        public override void _Ready()
        {
            CurveChanged += Refresh;
            Refresh();
        }

        private void Refresh()
        {
            while (GetChildCount() > 0)
            {
                var child = GetChild(0);
                RemoveChild(child);
                child.QueueFree();
            }

            if (RingPrefab == null)
                return;

            float length = Curve.GetBakedLength();
            float interval = length / (RingCount - 1);

            for (int i = 0; i < RingCount; i++)
            {
                CreateRing(Curve.SampleBakedWithRotation(interval * i));
            }
        }

        private void CreateRing(Transform3D transform)
        {
            var node = RingPrefab.Instantiate<Node3D>();
            node.Transform = transform;

            AddChild(node);
        }
    }
}