using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace pixel_miner.Components.Rendering
{
    public class HUDCamera : Camera
    {
        public override View GetView()
        {
            var center = new Vector2f(ViewSize.X / 2, ViewSize.Y / 2);
            var view = new View(center, ViewSize);
            view.Rotation = 0f;

            return view;
        }
    }
}