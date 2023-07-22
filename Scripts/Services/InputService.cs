using System;
using Godot;

namespace FlightSpeedway
{
    public static class InputService
    {
        public static Vector2 LeftStick => new Vector2(
            Input.GetAxis("LeftStickLeft", "LeftStickRight"),
            Input.GetAxis("LeftStickDown", "LeftStickUp")
        );
    }
}