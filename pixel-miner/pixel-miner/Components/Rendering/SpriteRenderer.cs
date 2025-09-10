using pixel_miner.Core;
using pixel_miner.Core.Enums;
using SFML.Graphics;
using SFML.System;

namespace pixel_miner.Components.Rendering
{
    public class SpriteRenderer : Component
    {
        public Color Color { get; set; } = Color.White;
        public Vector2f Size { get; set; } = new Vector2f(32f, 32f);
        public Texture? Texture { get; set; }

        private RectangleShape shape;
        private Sprite? sprite;

        public SpriteRenderer()
        {
            shape = new RectangleShape();
        }

        public SpriteRenderer(Color color, Vector2f size)
        {
            Color = color;
            Size = size;
            shape = new RectangleShape();
        }

        public override void Start()
        {
            RenderLayer = RenderLayer.World;
            UpdateShape();
        }

        public override void Update(float deltaTime)
        {
            if (Transform != null)
            {
                if (Texture != null && sprite != null)
                {
                    sprite.Position = Transform.Position;
                    sprite.Rotation = Transform.Rotation;
                    sprite.Scale = Transform.Scale;
                }
                else
                {
                    shape.Position = Transform.Position;
                    shape.Rotation = Transform.Rotation;
                    shape.Scale = Transform.Scale;
                }
            }
        }

        public override void Render(RenderWindow window)
        {
            if (Texture != null && sprite != null)
            {
                window.Draw(sprite);
            }
            else
            {
                window.Draw(shape);
            }
        }

        private void UpdateShape()
        {
            shape.Size = Size;
            shape.FillColor = Color;

            shape.Origin = new Vector2f(Size.X / 2f, Size.Y / 2f);

            if (Texture != null)
            {
                sprite = new Sprite(Texture);
                sprite.Origin = new Vector2f(Texture.Size.X / 2f, Texture.Size.Y / 2f);
            }
        }

        public void SetTexture(Texture texture)
        {
            Texture = texture;
            UpdateShape();
        }

        public void SetColor(Color color)
        {
            Color = color;
            UpdateShape();
        }

        public void SetSize(Vector2f size)
        {
            Size = size;
            UpdateShape();
        }
    }
}