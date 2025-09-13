using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace pixel_miner.Components.UI
{
    public class UISprite : UIElement
    {
        private RectangleShape shape;

        public Color Color { get; set; } = Color.White;

        public UISprite()
        {
            shape = new RectangleShape();
        }

        public UISprite(Color color) : this()
        {
            Color = color;
        }

        public override void Start()
        {
            base.Start();
            UpdateRenderObject();
        }

        public void SetColor(Color color)
        {
            Color = color;
            UpdateRenderObject();
        }

        private void UpdateRenderObject()
        {
            shape.Size = GetSize();
            shape.FillColor = Color;
            shape.Origin = new Vector2f(GetSize().X / 2, GetSize().Y / 2);
        }

        public override void Update(float deltaTime)
        {
            if (!Enabled) return;

            shape.Position = GetPosition();
            shape.Size = GetSize();
            shape.FillColor = Color;
            shape.Origin = new Vector2f(shape.Size.X / 2, shape.Size.Y / 2);
        }

        public override void Render(RenderWindow window)
        {
            if (!Enabled) return;

            window.Draw(shape);
        }

        public override void Destroy()
        {
            shape?.Dispose();
        }
    }
}