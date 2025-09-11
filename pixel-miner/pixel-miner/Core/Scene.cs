using pixel_miner.Components.Rendering;
using pixel_miner.Core.Enums;
using SFML.Graphics;

namespace pixel_miner.Core
{
    public class Scene
    {
        public string Name { get; set; }
        public bool Active { get; set; } = true;

        private List<GameObject> gameObjects = new List<GameObject>();
        private List<GameObject> objectsToAdd = new List<GameObject>();
        private List<GameObject> objectsToRemove = new List<GameObject>();

        public Scene(string name)
        {
            Name = name;
        }

        public void AddGameObject(GameObject gameObject)
        {
            objectsToAdd.Add(gameObject);
        }

        public void RemoveGameObject(GameObject gameObject)
        {
            objectsToRemove.Add(gameObject);
        }

        public GameObject? FindGameObject(string name)
        {
            return gameObjects.FirstOrDefault(go => go.Name == name);
        }

        public List<GameObject> FindGameObjectsWithComponent<T>() where T : Component
        {
            return gameObjects.Where(go => go.GetComponent<T>() != null).ToList();
        }

        public List<GameObject> GetAllGameObjects()
        {
            return new List<GameObject>(gameObjects);
        }

        public virtual void OnSceneStart()
        {
            foreach (var gameObject in gameObjects)
            {
                gameObject.ParentScene = this;
                gameObject.Start();
            }
        }

        public virtual void OnSceneEnd()
        {
            // Override in derived classes for cleanup
        }

        public void Update(float deltaTime)
        {
            if (!Active) return;

            ProcessPendingChanges();

            foreach (var gameObject in gameObjects)
            {
                gameObject.Update(deltaTime);
            }
        }

        public void Render(RenderWindow window)
        {
            if (!Active) return;

            var mainCamera = CameraManager.GetMainCamera();
            if (mainCamera != null)
            {
                window.SetView(mainCamera.GetView());
                RenderOnLayer(window, RenderLayer.World);
            }

            var hudCamera = CameraManager.GetCamera("HUD");
            if (hudCamera != null)
            {
                window.SetView(hudCamera.GetView());
                RenderOnLayer(window, RenderLayer.UI);
            }
        }

        private void RenderOnLayer(RenderWindow window, RenderLayer layer)
        {
            var renderersToRender = new List<Renderer>();

            foreach (var gameObject in gameObjects)
            {
                if (gameObject.Active)
                {
                    var renderers = gameObject.GetComponents<Renderer>()
                    .Where(r => r.Enabled && r.RenderLayer == layer)
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

        public void Destroy()
        {
            foreach (var gameObject in gameObjects)
            {
                gameObject.Destroy();
            }

            gameObjects.Clear();
            objectsToAdd.Clear();
            objectsToRemove.Clear();
        }

        private void ProcessPendingChanges()
        {
            AddGameObjects();
            RemoveGameObjects();
        }

        private void AddGameObjects()
        {
            foreach (var gameObject in objectsToAdd)
            {
                gameObjects.Add(gameObject);
                gameObject.ParentScene = this;
                gameObject.Start();
            }
            objectsToAdd.Clear();
        }

        private void RemoveGameObjects()
        {
            foreach (var gameObject in objectsToRemove)
            {
                gameObject.Destroy();
                gameObjects.Remove(gameObject);
            }
            objectsToRemove.Clear();
        }
    }
}