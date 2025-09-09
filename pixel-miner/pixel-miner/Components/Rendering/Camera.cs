using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using pixel_miner.Core;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace pixel_miner.Components.Rendering
{
    public class Camera : Component
    {
        public GameObject? Target { get; set; }
        public float FollowSpeed { get; set; } = 5f;
        public Vector2f Bounds { get; set; } = new Vector2f(float.MaxValue, float.MaxValue);
        public Vector2f ViewSize { get; set; }

        private RenderWindow? window;

        public Camera()
        {
            ViewSize = new Vector2f(1024f, 768f);
        }

        public void SetWindow(RenderWindow renderWindow)
        {
            window = renderWindow;
            ViewSize = new Vector2f(window.Size.X, window.Size.Y);
        }

        public override void Update(float deltaTime)
        {
            if (Transform == null) return;

            if (Target != null && Target.Transform != null)
            {
                Vector2f targetPos = Target.Transform.Position;
                Vector2f currentPos = Transform.Position;

                Vector2f newPos = Lerp(currentPos, targetPos, FollowSpeed * deltaTime);

                if (Bounds.X != float.MaxValue)
                {
                    newPos.X = Math.Max(-Bounds.X / 2f, Math.Min(Bounds.X / 2f, newPos.X));
                }

                if (Bounds.Y != float.MaxValue)
                {
                    newPos.Y = Math.Max(-Bounds.Y / 2f, Math.Min(Bounds.Y / 2f, newPos.Y));
                }

                Transform.Position = newPos;
            }
        }

        public Vector2f WorldToScreen(Vector2f worldPos)
        {
            if (Transform == null) return worldPos;

            Vector2f relativePos = worldPos - Transform.Position;

            relativePos.X *= Transform.Scale.X;
            relativePos.Y *= Transform.Scale.Y;

            if (Transform.Rotation != 0)
            {
                float cos = (float)Math.Cos(Transform.Rotation * Math.PI / 180f);
                float sin = (float)Math.Sin(Transform.Rotation * Math.PI / 180f);

                float rotX = relativePos.X * cos - relativePos.Y * sin;
                float rotY = relativePos.X * sin + relativePos.Y * cos;

                relativePos.X = rotX;
                relativePos.Y = rotY;
            }

            return new Vector2f(
                ViewSize.X / 2 + relativePos.X,
                ViewSize.Y / 2f + relativePos.Y
            );
        }

        public Vector2f ScreenToWorld(Vector2f screenPos)
        {
            if (Transform == null) return screenPos;

            Vector2f relativePos = new Vector2f(
                screenPos.X - ViewSize.X / 2f,
                screenPos.Y - ViewSize.Y / 2f
            );

            if (Transform.Rotation != 0)
            {
                float cos = (float)Math.Cos(-Transform.Rotation * Math.PI / 180f);
                float sin = (float)Math.Sin(-Transform.Rotation * Math.PI / 180f);

                float rotX = relativePos.X * cos - relativePos.Y * sin;
                float rotY = relativePos.X * sin + relativePos.Y * cos;

                relativePos.X = rotX;
                relativePos.Y = rotY;
            }

            relativePos.X /= Transform.Scale.X;
            relativePos.Y /= Transform.Scale.Y;

            return Transform.Position + relativePos;
        }

        public Vector2f GetMouseWorldPosition()
        {
            if (window == null) return new Vector2f(0, 0);

            Vector2i mousePos = Mouse.GetPosition(window);
            Vector2f screenPos = new Vector2f(mousePos.X, mousePos.Y);
            return ScreenToWorld(screenPos);
        }

        public View GetView()
        {
            if (Transform == null) return new View();

            var view = new View(Transform.Position, ViewSize);
            view.Zoom(1f / Transform.Scale.X);
            view.Rotation = Transform.Rotation;

            return view;
        }

        public void SetTarget(GameObject target)
        {
            Target = target;
        }

        private Vector2f Lerp(Vector2f a, Vector2f b, float t)
        {
            t = Math.Max(0f, Math.Min(1f, t));
            return new Vector2f(
                a.X + (b.X - a.X) * t,
                a.Y + (b.Y - a.Y) * t
            );
        }
    }
}