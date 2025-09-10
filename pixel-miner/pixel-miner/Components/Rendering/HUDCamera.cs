using pixel_miner.Core.Enums;
using SFML.Graphics;
using SFML.System;

namespace pixel_miner.Components.Rendering
{
    public class HUDCamera : Camera
    {
        public override void Start()
        {
            RenderLayer = RenderLayer.World;
        }
        public override View GetView()
        {
            var center = new Vector2f(ViewSize.X / 2, ViewSize.Y / 2);
            var view = new View(center, ViewSize);
            view.Rotation = 0f;

            return view;
        }
    }
}