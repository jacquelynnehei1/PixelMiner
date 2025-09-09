using SFML.System;

namespace pixel_miner.Utils.Extensions
{
    public static class Vector2fExtensions
    {
        public static float Distance(this Vector2f a, Vector2f b)
        {
            float dx = b.X - a.X;
            float dy = b.Y - a.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        public static Vector2f Lerp(this Vector2f a, Vector2f b, float t)
        {
            t = Math.Max(0f, Math.Min(1f, t));

            return new Vector2f(
                a.X + (b.X - a.X) * t,
                a.Y + (b.Y - a.Y) * t
            );
        }

        public static float Magnitude(this Vector2f vector)
        {
            return (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
        }

        public static Vector2f Normalized(this Vector2f vector)
        {
            float magnitude = vector.Magnitude();

            if (magnitude == 0f)
            {
                return new Vector2f(0f, 0f);
            }

            return new Vector2f(vector.X / magnitude, vector.Y / magnitude);
        }
    }
}