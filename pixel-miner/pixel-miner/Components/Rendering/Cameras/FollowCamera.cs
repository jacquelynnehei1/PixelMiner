using pixel_miner.Core;
using pixel_miner.Core.Enums;
using pixel_miner.Utils.Extensions;
using SFML.Graphics;
using SFML.System;

namespace pixel_miner.Components.Rendering.Cameras
{
    public class FollowCamera : Camera
    {
        public GameObject? Target { get; set; }
        public float FollowSpeed { get; set; } = 5f;

        public bool FollowX { get; set; } = true;
        public bool FollowY { get; set; } = true;

        public override void Start()
        {
            RenderLayer = RenderLayer.World;
        }

        public override void Update(float deltaTime)
        {
            if (Transform == null) return;

            if (Target != null && Target.Transform != null)
            {
                Vector2f targetPos = Target.Transform.Position;
                Vector2f currentPos = Transform.Position;

                Vector2f newPos = currentPos.Lerp(targetPos, FollowSpeed * deltaTime);

                if (!FollowX)
                {
                    newPos.X = currentPos.X;
                }

                if (!FollowY)
                {
                    newPos.Y = currentPos.Y;
                }

                if (Bounds.X != float.MaxValue && FollowX)
                    {
                        newPos.X = Math.Max(-Bounds.X / 2f, Math.Min(Bounds.X / 2f, newPos.X));
                    }

                if (Bounds.Y != float.MaxValue && FollowY)
                {
                    newPos.Y = Math.Max(-Bounds.Y / 2f, Math.Min(Bounds.Y / 2f, newPos.Y));
                }

                Transform.Position = newPos;
            }
        }

        public void SetTarget(GameObject target)
        {
            Target = target;
            Transform.Position = target.Transform.Position;
        }

        public override void Render(RenderWindow window)
        {
            CameraManager.ApplyCameraToWindow(window, Name);
        }
    }
}