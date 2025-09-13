using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pixel_miner.Core;
using pixel_miner.Utils.Extensions;
using SFML.Graphics;

namespace pixel_miner.Components.UI
{
    public class UIText : UIElement
    {
        private Text textObject;

        public string DisplayText { get; set; } = "";
        public uint FontSize { get; set; } = 12;
        public Color TextColor { get; set; } = Color.White;
        public string FontPath { get; set; } = "Assets/Fonts/Default.otf";

        public UIText()
        {
            textObject = new Text();
        }

        public UIText(string text) : this()
        {
            DisplayText = text;
            textObject.DisplayedString = text;
        }

        public override void Start()
        {
            base.Start();

            textObject.DisplayedString = DisplayText;
            textObject.FillColor = TextColor;
            textObject.CharacterSize = FontSize;

            if (!FontPath.IsNullOrEmpty())
            {
                SetFont(FontPath);
            }
        }

        public void SetFont(string fontPath)
        {
            FontPath = fontPath;
            var font = FontManager.LoadFont(fontPath);
            if (font != null)
            {
                textObject.Font = font;
            }
        }

        public void SetText(string text)
        {
            DisplayText = text;
            textObject.DisplayedString = text;
        }

        public void SetColor(Color color)
        {
            TextColor = color;
            textObject.FillColor = color;
        }

        public void SetFontSize(uint size)
        {
            FontSize = size;
            textObject.CharacterSize = size;
        }

        public override void Update(float deltaTime)
        {
            if (!Enabled) return;

            textObject.Position = GetPosition();
            textObject.DisplayedString = DisplayText;
            textObject.FillColor = TextColor;
            textObject.CharacterSize = FontSize;
        }

        public override void Render(RenderWindow window)
        {
            if (!Enabled) return;

            window.Draw(textObject);
        }

        public override void Destroy()
        {
            textObject?.Dispose();
        }
    }
}