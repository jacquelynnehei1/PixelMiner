using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFML.Graphics;

namespace pixel_miner.Core
{
    public class SceneManager
    {
        private static Dictionary<string, Scene> scenes = new Dictionary<string, Scene>();
        private static Scene? currentScene = null;
        private static Scene? nextScene = null;

        public static void AddScene(Scene scene)
        {
            scenes[scene.Name] = scene;
        }

        public static void RemoveScene(string sceneName)
        {
            if (scenes.ContainsKey(sceneName))
            {
                if (currentScene?.Name == sceneName)
                {
                    currentScene = null;
                }

                scenes[sceneName].Destroy();
                scenes.Remove(sceneName);
            }
        }

        public static void LoadScene(string sceneName)
        {
            if (scenes.ContainsKey(sceneName))
            {
                nextScene = scenes[sceneName];
            }
        }

        public static Scene? GetCurrentScene()
        {
            return currentScene;
        }

        public static Scene? GetScene(string sceneName)
        {
            return scenes.ContainsKey(sceneName) ? scenes[sceneName] : null;
        }

        public static void Update(float deltaTime)
        {
            if (nextScene != null)
            {
                currentScene?.OnSceneEnd();
                currentScene = nextScene;
                currentScene.OnSceneStart();
                nextScene = null;
            }

            currentScene?.Update(deltaTime);
        }

        public static void Render(RenderWindow window)
        {
            currentScene?.Render(window);
        }

        public static void Destroy()
        {
            foreach (var scene in scenes.Values)
            {
                scene.Destroy();
            }

            scenes.Clear();
            currentScene = null;
            nextScene = null;
        }
    }
}