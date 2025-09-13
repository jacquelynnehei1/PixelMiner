using pixel_miner.Components.UI.Enums;
using pixel_miner.Core;
using SFML.System;

namespace pixel_miner.Components.UI
{
    public class UITransform : Component
    {
        public Vector2f AnchorMin { get; set; } = new Vector2f(0, 0);
        public Vector2f AnchorMax { get; set; } = new Vector2f(0, 0);

        public Vector2f AnchoredPosition { get; set; } = new Vector2f(0, 0);
        public Vector2f SizeDelta { get; set; } = new Vector2f(100, 30);
        public UITransform? Parent { get; set; }
        public UICanvas Canvas { get; set; }

        public UITransform()
        {
            SetAnchor(AnchorPreset.TopLeft);
        }

        public void SetAnchor(AnchorPreset preset)
        {
            switch (preset)
            {
                case AnchorPreset.TopLeft:
                    AnchorMin = new Vector2f(0, 0);
                    AnchorMax = new Vector2f(0, 0);
                    break;
                case AnchorPreset.TopCenter:
                    AnchorMin = new Vector2f(0.5f, 0);
                    AnchorMax = new Vector2f(0.5f, 0);
                    break;
                case AnchorPreset.TopRight:
                    AnchorMin = new Vector2f(1, 0);
                    AnchorMax = new Vector2f(1, 0);
                    break;
                case AnchorPreset.MiddleLeft:
                    AnchorMin = new Vector2f(0, 0.5f);
                    AnchorMax = new Vector2f(0, 0.5f);
                    break;
                case AnchorPreset.MiddleCenter:
                    AnchorMin = new Vector2f(0.5f, 0.5f);
                    AnchorMax = new Vector2f(0.5f, 0.5f);
                    break;
                case AnchorPreset.MiddleRight:
                    AnchorMin = new Vector2f(1, 0.5f);
                    AnchorMax = new Vector2f(1, 0.5f);
                    break;
                case AnchorPreset.BottomLeft:
                    AnchorMin = new Vector2f(0, 1);
                    AnchorMax = new Vector2f(0, 1);
                    break;
                case AnchorPreset.BottomCenter:
                    AnchorMin = new Vector2f(0.5f, 1);
                    AnchorMax = new Vector2f(0.5f, 1);
                    break;
                case AnchorPreset.BottomRight:
                    AnchorMin = new Vector2f(1, 1);
                    AnchorMax = new Vector2f(1, 1);
                    break;
                case AnchorPreset.StretchHorizontal:
                    AnchorMin = new Vector2f(0, 0.5f);
                    AnchorMax = new Vector2f(1, 0.5f);
                    break;
                case AnchorPreset.StretchVertical:
                    AnchorMin = new Vector2f(0.5f, 0);
                    AnchorMax = new Vector2f(0.5f, 1);
                    break;
                case AnchorPreset.StretchBoth:
                    AnchorMin = new Vector2f(0, 0);
                    AnchorMax = new Vector2f(1, 1);
                    break;
            }
        }

        public Vector2f GetWorldPosition()
        {
            Vector2f parentSize = GetParentSize();
            Vector2f parentPosition = GetParentPosition();

            Vector2f anchorPosition = new Vector2f(
                parentPosition.X + (AnchorMin.X * parentSize.X),
                parentPosition.Y + (AnchorMin.Y * parentSize.Y)
            );

            return anchorPosition + AnchoredPosition;
        }

        public Vector2f GetSize()
        {
            if (AnchorMin.X == AnchorMax.X && AnchorMin.Y == AnchorMax.Y)
            {
                return SizeDelta;
            }

            Vector2f parentSize = GetParentSize();

            float width = SizeDelta.X;
            float height = SizeDelta.Y;

            if (AnchorMin.X != AnchorMax.X)
            {
                width = parentSize.X * (AnchorMax.X - AnchorMin.X) + SizeDelta.X;
            }

            if (AnchorMin.Y != AnchorMax.Y)
            {
                height = parentSize.Y * (AnchorMax.Y - AnchorMin.Y) + SizeDelta.Y;
            }

            return new Vector2f(width, height);
        }

        public void SetAnchoredPosition(float x, float y)
        {
            AnchoredPosition = new Vector2f(x, y);
            UpdateTransform();
        }

        public void SetSize(float width, float height)
        {
            SizeDelta = new Vector2f(width, height);
        }

        public void UpdateTransform()
        {
            if (Transform != null)
            {
                Transform.Position = GetWorldPosition();
            }
        }

        public override void Update(float deltaTime)
        {
            UpdateTransform();
        }

        private Vector2f GetParentSize()
        {
            if (Parent != null)
            {
                return Parent.GetSize();
            }

            if (Canvas != null)
            {
                return Canvas.CanvasSize;
            }

            return new Vector2f(1920, 1080);
        }

        private Vector2f GetParentPosition()
        {
            if (Parent != null)
            {
                return Parent.GetWorldPosition();
            }

            return new Vector2f(0, 0);
        }
    }
}