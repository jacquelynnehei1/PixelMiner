using pixel_miner.Components.Rendering;
using pixel_miner.Utils.Extensions;
using SFML.Graphics;

namespace pixel_miner.Core
{
    public class CameraManager
    {
        private static Camera? mainCamera = null;
        private static Dictionary<string, Camera> cameras = new Dictionary<string, Camera>();

        public static void SetMainCamera(Camera camera)
        {
            mainCamera = camera;
        }

        public static void AddCamera(Camera camera, bool isMainCamera = false)
        {
            if (!cameras.ContainsKey(camera.Name))
            {
                cameras.Add(camera.Name, camera);

                if (isMainCamera)
                {
                    mainCamera = camera;
                }
            }
        }

        public static Camera? GetMainCamera()
        {
            return mainCamera;
        }

        public static Camera? GetCamera(string cameraName)
        {
            return cameras.ContainsKey(cameraName) ? cameras[cameraName] : null;
        }

        public static void ApplyCameraToWindow(RenderWindow window, string cameraName = "")
        {
            if (!cameraName.IsNullOrEmpty() && cameras.ContainsKey(cameraName))
            {
                Camera camera = cameras[cameraName];
                window.SetView(camera.GetView());
                return;
            }

            if (mainCamera != null)
            {
                window.SetView(mainCamera.GetView());
                return;
            }
        }

        public static void ResetView(RenderWindow window)
        {
            window.SetView(window.GetView());
        }
    }
}