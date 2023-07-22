using Godot;
namespace FlightSpeedway
{
    public static class MathUtils
    {
        public static float DecayToward(
            float from,
            float to,
            float decayRate,
            float delta
        )
        {
            float remaining = Mathf.Abs(from - to);
            remaining *= Mathf.Pow(Mathf.E, -decayRate * delta);
            return Mathf.MoveToward(from, to, remaining);
        }

        public static Vector3 DecayToward(
            Vector3 from,
            Vector3 to,
            float decayRate,
            float delta
        )
        {
            float remaining = from.DistanceTo(to);
            remaining = DecayToward(remaining, 0, decayRate, 0);
            return to.MoveToward(from, remaining);
        }
    }
}