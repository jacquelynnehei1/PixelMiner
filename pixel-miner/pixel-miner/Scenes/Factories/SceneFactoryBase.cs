using pixel_miner.Components.Rendering;
using pixel_miner.Core;
using SFML.Graphics;
using SFML.System;

namespace pixel_miner.Scenes.Factories
{
    public abstract class SceneFactoryBase : ISceneFactory
    {
        public abstract string SceneName { get; }

        public abstract Scene CreateScene();

        protected GameObject CreateBasicCamera(GameObject? target = null)
        {
            var cameraObject = new GameObject("MainCamera", 0, 0);
            var camera = cameraObject.AddComponent<Camera>();
            CameraManager.AddCamera(camera, isMainCamera: true);
            return cameraObject;
        }

        protected GameObject CreateBasicPlayer(float x = 0, float y = 0)
        {
            var player = new GameObject("Player", x, y);

            var spriteRenderer = player.AddComponent<SpriteRenderer>();
            spriteRenderer.SetColor(Color.Red);
            spriteRenderer.SetSize(new Vector2f(32f, 32f));

            return player;
        }

        protected GameObject CreateTestObject(string name, float x, float y, Color color)
        {
            var obj = new GameObject(name, x, y);

            var spriteRenderer = obj.AddComponent<SpriteRenderer>();
            spriteRenderer.SetColor(color);
            spriteRenderer.SetSize(new Vector2f(16f, 16f));

            return obj;
        }
    }
}