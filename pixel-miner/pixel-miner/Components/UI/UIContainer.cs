using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pixel_miner.Core;
using SFML.Graphics;

namespace pixel_miner.Components.UI
{
    public class UIContainer : UIElement
    {
        protected List<UIElement> childElements = new List<UIElement>();
        private RectangleShape? background;

        public Color? BackgroundColor { get; set; }

        public void AddChild(UIElement childElement)
        {
            if (childElement.GameObject == null)
            {
                var childObject = new GameObject($"{GameObject.Name}_Child");
                childObject.AddComponent(childElement);
            }

            childElements.Add(childElement);

            if (childElement.UITransform != null)
            {
                childElement.UITransform.Parent = this.UITransform;
            }

            if (canvas != null)
            {
                childElement.SetCanvas(canvas);
                canvas.RegisterUIElement(childElement);
            }

            GameObject.Create(childElement.GameObject);
        }

        public void RemoveChild(UIElement childElement)
        {
            childElements.Remove(childElement);
            if (childElement.GameObject != null)
            {
                canvas?.UnregisterUIElement(childElement);
                GameObject.Destroy(childElement.GameObject);
            }
        }

        public void SetBackground(Color color)
        {
            BackgroundColor = color;
            if (background == null)
            {
                background = new RectangleShape();
            }

            background.Size = GetSize();
            background.Position = GetPosition();
            background.FillColor = color;
        }

        public override void Update(float deltaTime)
        {
            if (!Enabled) return;

            if (background != null)
            {
                background.Position = GetPosition();
                background.Size = GetSize();
            }
        }

        public override void Render(RenderWindow window)
        {
            if (!Enabled) return;

            if (background != null && BackgroundColor.HasValue)
            {
                window.Draw(background);
            }
        }

        public override void Destroy()
        {
            foreach (var child in childElements)
            {
                if (child.GameObject != null)
                {
                    GameObject.Destroy(child.GameObject);
                }
            }

            childElements.Clear();
            background?.Dispose();
        }
    }
}