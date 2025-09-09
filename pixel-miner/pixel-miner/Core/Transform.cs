using SFML.System;

namespace pixel_miner.Core
{
    public class Transform : Component
    {
        public Vector2f Position { get; set; }
        public float Rotation { get; set; }
        public Vector2f Scale { get; set; }

        public Transform()
        {
            Position = new Vector2f(0f, 0f);
            Rotation = 0f;
            Scale = new Vector2f(1f, 1f);
        }

        public Transform(Vector2f position)
        {
            Position = position;
            Rotation = 0f;
            Scale = new Vector2f(1f, 1f);
        }

        public Transform(float x, float y)
        {
            Position = new Vector2f(x, y);
            Rotation = 0f;
            Scale = new Vector2f(1f, 1f);
        }
    }
}