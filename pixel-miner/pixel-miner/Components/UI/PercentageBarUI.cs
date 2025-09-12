using pixel_miner.Components.Rendering;
using pixel_miner.Core;
using pixel_miner.Core.Enums;
using pixel_miner.Utils.Extensions;
using SFML.Graphics;
using SFML.System;

namespace pixel_miner.Components.UI
{
    public class PercentageBarUI : Renderer
    {
        // Visual Elements
        private RectangleShape backgroundBar = null!;
        private RectangleShape fillBar = null!;
        private Text labelText = null!;

        // Configuration
        public Vector2f Size { get; set; } = new Vector2f(200f, 20f);
        public Color BackgroundColor { get; set; } = new Color(255, 255, 255);
        public Color FillColor { get; set; } = new Color(237, 161, 47);
        public Color TextColor { get; set; } = Color.Black;
        public bool ShowText { get; set; } = true;
        public string Label { get; set; } = "";
        public string FontPath { get; set; } = "";
        public Vector2f LabelOffset = new Vector2f(0f, 0f);

        // State
        private float currentPercentage = 1.0f;

        public PercentageBarUI()
        {
            InitializeVisualElements();
        }

        public override void Start()
        {
            UpdateVisualElements();
            Console.WriteLine($"Font is null or empty? {FontPath.IsNullOrEmpty()}");
        }

        public override void Render(RenderWindow window)
        {
            window.Draw(backgroundBar);
            window.Draw(fillBar);

            if (ShowText && !FontPath.IsNullOrEmpty())
            {
                window.Draw(labelText);
            }
        }

        /// <summary>
        /// Update the percentage (0.0 to 1.0)
        /// </summary>
        /// <param name="percentage"></param>
        public virtual void SetPercentage(float percentage)
        {
            currentPercentage = Math.Max(0f, Math.Min(1f, percentage));
            UpdateFillBar();
        }

        /// <summary>
        /// Updates the fill color
        /// </summary>
        /// <param name="color"></param>
        public virtual void SetFillColor(Color color)
        {
            FillColor = color;
            fillBar.FillColor = color;
        }

        /// <summary>
        /// Sets the font for text rendering
        /// </summary>
        /// <param name="gameFont"></param>
        public void SetFont(string fontPath)
        {
            FontPath = fontPath;
            labelText.Font = FontManager.LoadFont(fontPath);
        }

        /// <summary>
        /// Sets the label text
        /// </summary>
        /// <param name="label"></param>
        public void SetLabel(string label)
        {
            Label = label;
            labelText.DisplayedString = label;
        }

        public void SetLabelOffset(Vector2f offset)
        {
            LabelOffset = offset;
        }

        /// <summary>
        /// Configure the bar's appearance and position
        /// </summary>
        /// <param name="position"></param>
        /// <param name="size"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="fillColor"></param>
        /// <param name="showText"></param>
        public void Configure(Vector2f size, Color backgroundColor, Color fillColor, Color textColor, bool showText = true)
        {
            Size = size;
            BackgroundColor = backgroundColor;
            FillColor = fillColor;
            TextColor = textColor;
            ShowText = showText;

            UpdateVisualElements();
        }

        private void InitializeVisualElements()
        {
            backgroundBar = new RectangleShape();
            fillBar = new RectangleShape();

            labelText = new Text();
            labelText.DisplayedString = "Test";
            labelText.CharacterSize = 24;
            labelText.FillColor = Color.White;

            if (!FontPath.IsNullOrEmpty())
            {
                labelText.Font = FontManager.LoadFont(FontPath);
            }
        }

        private void UpdateVisualElements()
        {
            if (Transform == null) return;

            var position = Transform.Position;

            backgroundBar.Size = Size;
            backgroundBar.Position = position;
            backgroundBar.FillColor = BackgroundColor;

            UpdateFillBar();

            var labelPosition = position;
            labelPosition += LabelOffset;

            labelText.DisplayedString = Label;
            labelText.Origin = new Vector2f(labelText.GetLocalBounds().Width / 2, labelText.GetLocalBounds().Height / 2);
            labelText.Position = labelPosition;
        }

        private void UpdateFillBar()
        {
            float currentWidth = Size.X * currentPercentage;
            fillBar.Size = new Vector2f(currentWidth, Size.Y);
            fillBar.Position = Transform.Position;
            fillBar.FillColor = FillColor;
        }
    }
}