using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pixel_miner.Components.Rendering;
using SFML.Graphics;

namespace pixel_miner.Core
{
    public class CameraManager
    {
        private static Camera? mainCamera = null;

        public static void SetMainCamera(Camera camera)
        {
            mainCamera = camera;
        }

        public static Camera? GetMainCamera()
        {
            return mainCamera;
        }

        public static void ApplyCameraToWindow(RenderWindow window)
        {
            if (mainCamera != null)
            {
                window.SetView(mainCamera.GetView());
            }
        }

        public static void ResetView(RenderWindow window)
        {
            window.SetView(window.GetView());
        }
    }
}