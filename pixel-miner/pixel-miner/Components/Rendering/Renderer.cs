using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pixel_miner.Core;
using pixel_miner.Core.Enums;
using SFML.Graphics;

namespace pixel_miner.Components.Rendering
{
    public abstract class Renderer : Component
    {
        /// <summary>
        /// The rendering layer
        /// </summary>
        public RenderLayer RenderLayer { get; set; } = RenderLayer.World;

        /// <summary>
        /// Sorting order iwthin the render layer. Higher values on top.
        /// </summary>
        public int SortingOrder { get; set; } = 0;

        public abstract override void Render(RenderWindow window);
    }
}