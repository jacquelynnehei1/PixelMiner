using pixel_miner.Core;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace pixel_miner.Components.Rendering
{
    public class Camera : Component
    {
        public string Name { get; set; }
        public Vector2f Bounds { get; set; } = new Vector2f(float.MaxValue, float.MaxValue);
        public Vector2f ViewSize { get; set; }

        private RenderWindow? window;

        public Camera()
        {
            Name = "Camera";
            ViewSize = new Vector2f(1024f, 768f);
        }

        public Camera(string cameraName, Vector2f viewSize)
        {
            Name = cameraName;
            ViewSize = viewSize;
        }

        public void SetWindow(RenderWindow renderWindow)
        {
            window = renderWindow;
            ViewSize = new Vector2f(window.Size.X, window.Size.Y);
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

        public void SetName(string cameraName)
        {
            Name = cameraName;
        }
    }
}