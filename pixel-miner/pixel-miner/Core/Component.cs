using pixel_miner.Core.Enums;
using SFML.Graphics;

namespace pixel_miner.Core
{
    public abstract class Component
    {
        public GameObject GameObject { get; internal set; } = null!;
        public Transform Transform => GameObject?.GetComponent<Transform>()!;
        public bool Enabled { get; set; } = true;
        public RenderLayer RenderLayer { get; set; } = RenderLayer.World;

        /// <summary>
        /// Called when component is added to GameObject
        /// </summary>
        public virtual void Start() { }

        /// <summary>
        /// Called every frame
        /// </summary>
        public virtual void Update(float deltaTime) { }

        /// <summary>
        /// Called every frame for rendering
        /// </summary>
        /// <param name="window"></param>
        public virtual void Render(RenderWindow window) { }

        /// <summary>
        /// Called when component is removed
        /// </summary>
        public virtual void Destroy() { }
    }
}
