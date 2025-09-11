using SFML.Graphics;
using SFML.System;

namespace pixel_miner.Core
{
    public class GameObject
    {
        public string Name { get; set; }
        public bool Active { get; set; } = true;
        public Transform Transform { get; set; }


        internal Scene? ParentScene { get; set; }

        private List<Component> components = new List<Component>();
        private bool started = false;

        public GameObject(String name = "GameObject")
        {
            Name = name;
            Transform = AddComponent<Transform>();
        }

        public GameObject(string name, float x, float y)
        {
            Name = name;
            Transform = AddComponent<Transform>();
            Transform.Position = new Vector2f(x, y);
        }

        public T AddComponent<T>() where T : Component, new()
        {
            var component = new T();
            AddComponent(component);
            return component;
        }

        public void AddComponent(Component component)
        {
            component.GameObject = this;
            components.Add(component);

            if (started)
            {
                component.Start();
            }
        }

        /// <summary>
        /// Get a component of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T? GetComponent<T>() where T : Component
        {
            return components.OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// Get all components of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> GetComponents<T>() where T : Component
        {
            return components.OfType<T>().ToList();
        }

        /// <summary>
        /// Remove a component
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RemoveComponent<T>() where T : Component
        {
            var component = GetComponent<T>();
            if (component != null)
            {
                component.Destroy();
                components.Remove(component);
            }
        }

        public static GameObject? Find(string name)
        {
            var currentScene = SceneManager.GetCurrentScene();
            return currentScene?.FindGameObject(name);
        }

        public static GameObject? FindObjectOfType<T>() where T : Component
        {
            var currentScene = SceneManager.GetCurrentScene();
            return currentScene?.FindGameObjectsWithComponent<T>().FirstOrDefault();
        }

        public static List<GameObject> FindObjectsOfType<T>() where T : Component
        {
            var currentScene = SceneManager.GetCurrentScene();
            return currentScene?.FindGameObjectsWithComponent<T>() ?? new List<GameObject>();
        }

        public static T? FindObjectOfType<T>(string name) where T : Component
        {
            var gameObject = Find(name);
            return gameObject?.GetComponent<T>();
        }

        public static List<GameObject> FindAllObjects()
        {
            var currentScene = SceneManager.GetCurrentScene();
            return currentScene?.GetAllGameObjects() ?? new List<GameObject>();
        }

        public static void Create(GameObject gameObject)
        {
            var currentScene = SceneManager.GetCurrentScene();
            currentScene?.AddGameObject(gameObject);
        }

        public static void Destroy(GameObject gameObject)
        {
            var currentScene = SceneManager.GetCurrentScene();
            currentScene?.RemoveGameObject(gameObject);
        }

        /// <summary>
        /// Called once when GameObject is first created/added to the scene
        /// </summary>
        public void Start()
        {
            if (started) return;

            foreach (var component in components)
            {
                component.Start();
            }

            started = true;
        }

        /// <summary>
        /// Update all components, called once per frame
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Update(float deltaTime)
        {
            if (!Active) return;

            foreach (var component in components)
            {
                if (component.Enabled)
                {
                    component.Update(deltaTime);
                }
            }
        }

        /// <summary>
        /// Render all components, called once per frame
        /// </summary>
        /// <param name="window"></param>
        public void Render(RenderWindow window)
        {
            if (!Active) return;

            foreach (var component in components)
            {
                if (component.Enabled)
                {
                    component.Render(window);
                }
            }
        }

        /// <summary>
        /// Called when a GameObject is removed from the scene
        /// </summary>
        public void Destroy()
        {
            foreach (var component in components)
            {
                component.Destroy();
            }

            components.Clear();
            ParentScene = null;
        }
    }
}
