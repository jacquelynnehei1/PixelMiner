using pixel_miner.Components.Rendering;
using pixel_miner.Components.UI.Enums;
using pixel_miner.Core.Enums;
using SFML.Graphics;
using SFML.System;

namespace pixel_miner.Components.UI
{
    public abstract class UIElement : Renderer
    {
        public UITransform UITransform { get; private set; } = null!;
        protected UICanvas? canvas;

        public UIElement()
        {
            RenderVeiw = RenderView.RightPanel;
            SortingOrder = 0;
        }

        public override void Start()
        {
            EnsureUITransformExists();
        }

        private void EnsureUITransformExists()
        {
            if (UITransform == null && GameObject != null)
            {
                var existingTransform = GameObject.GetComponent<UITransform>();
                if (existingTransform != null)
                {
                    UITransform = existingTransform;
                    return;
                }

                UITransform = GameObject.AddComponent<UITransform>();
                UITransform.Start();
            }
        }

        public void SetCanvas(UICanvas targetCanvas)
        {
            canvas = targetCanvas;
            EnsureUITransformExists();
            UITransform.Canvas = targetCanvas;
            RenderVeiw = targetCanvas.TargetView;
        }

        public Vector2f GetPosition() => UITransform?.GetWorldPosition() ?? new Vector2f(0, 0);
        public Vector2f GetSize() => UITransform?.GetSize() ?? new Vector2f(100, 30);

        public void SetPosition(float x, float y) => UITransform?.SetAnchoredPosition(x, y);
        public void SetSize(float width, float height) => UITransform?.SetSize(width, height);
        public void SetAnchor(AnchorPreset preset) => UITransform?.SetAnchor(preset);

        public abstract override void Render(RenderWindow window);
    }
}