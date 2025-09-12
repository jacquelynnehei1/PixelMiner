using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pixel_miner.Components.Rendering.Cameras;
using pixel_miner.Core;
using SFML.System;

namespace pixel_miner.Tests
{
    public class TestCameraSetup
    {
        public static GameObject CreateTestCamera(float width = 1920f, float height = 1080)
        {
            var gameObject = new GameObject("TestCamera");
            var camera = gameObject.AddComponent<Camera>();

            camera.SetViewport(0.25f, 0f, 0.5f, 1f);
            camera.SetViewSize(new Vector2f(width, height));
            camera.SetName(gameObject.Name);

            CameraManager.AddCamera(camera, isMainCamera: true);

            return gameObject;
        }

        public static void CleanupCameras()
        {
            CameraManager.SetMainCamera(null);
        }
    }
}