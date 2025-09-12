using pixel_miner.Components.Rendering;
using pixel_miner.Components.Rendering.Cameras;
using pixel_miner.Core;
using pixel_miner.Core.Enums;
using SFML.Graphics;
using System.Linq;

namespace pixel_miner.Scenes
{
    public class MainGameScene : Scene
    {
        public Camera? leftCamera;
        public Camera? centerCamera;
        public Camera? rightCamera;

        public MainGameScene(string name) : base(name) { }

        public override void Render(RenderWindow window)
        {
            if (!Active) return;

            if (leftCamera != null)
            {
                window.SetView(leftCamera.GetView());
                RenderOnView(window, RenderView.LeftPanel);
            }

            if (centerCamera != null)
            {
                window.SetView(centerCamera.GetView());
                RenderOnView(window, RenderView.CenterPanel);
            }

            if (rightCamera != null)
            {
                window.SetView(rightCamera.GetView());
                RenderOnView(window, RenderView.RightPanel);
            }
        }

        private void RenderOnView(RenderWindow window, RenderView renderView)
        {
            var renderersToRender = new List<Renderer>();

            foreach (var gameObject in gameObjects)
            {
                if (gameObject.Active)
                {
                    var renderers = gameObject.GetComponents<Renderer>()
                    .Where(r => r.Enabled && r.RenderVeiw == renderView)
                    .ToList();

                    renderersToRender.AddRange(renderers);
                }
            }

            var sortedRenderers = renderersToRender.OrderBy(r => r.SortingOrder).ToList();

            foreach (var renderer in sortedRenderers)
            {
                renderer.Render(window);
            }
        }
    }
}