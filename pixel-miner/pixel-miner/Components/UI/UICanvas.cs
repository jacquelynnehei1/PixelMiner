using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pixel_miner.Core;
using pixel_miner.Core.Enums;
using SFML.System;

namespace pixel_miner.Components.UI
{
    public class UICanvas : Component
    {
        public Vector2f CanvasSize { get; set; }
        public RenderView TargetView { get; set; }

        private List<UIElement> childElements = new List<UIElement>();

        public UICanvas() { }

        public UICanvas(RenderView targetView, Vector2f canvasSize)
        {
            TargetView = targetView;
            CanvasSize = canvasSize;
        }

        public void RegisterUIElement(UIElement element)
        {
            if (!childElements.Contains(element))
            {
                childElements.Add(element);
                element.SetCanvas(this);
            }
        }

        public void UnregisterUIElement(UIElement element)
        {
            childElements.Remove(element);
        }

        public override void Update(float deltaTime)
        {
            // Canvas can handle input events, coordinate system updates, etc. 
            // child elements update themselves through normal component system
        }

        public override void Destroy()
        {
            childElements.Clear();
        }

        public Vector2f ScreenToCanvasPosition(Vector2f screenPosition)
        {
            // This would convert from full screen coordinates to this canvas's coordinate system
            // Implementation depends on how your camera viewports are set up
            return screenPosition;
        }

        public bool ContainsScreenPosition(Vector2f screenPosition)
        {
            // Implementation depends on viewport bounds
            return true;
        }
    }
}