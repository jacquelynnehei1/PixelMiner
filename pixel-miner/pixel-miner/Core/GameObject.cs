using SFML.Graphics;
using SFML.System;

namespace pixel_miner.Core
{
    public class GameObject
    {
        public string Name { get; set; }
        public bool Active { get; set; } = true;
        public Transform Transform { get; set; }

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
        }
    }
}
