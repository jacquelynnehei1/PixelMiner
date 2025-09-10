using SFML.Graphics;

namespace pixel_miner.Core
{
    public static class FontManager
    {
        private static Dictionary<string, Font> loadedFonts = new Dictionary<string, Font>();
        private static Font? defaultFont = null;

        public static Font? GetDefaultFont()
        {
            return defaultFont;
        }

        public static Font? LoadFont(string fontPath, bool makeDefault = false)
        {
            if (loadedFonts.ContainsKey(fontPath))
            {
                if (makeDefault)
                {
                    defaultFont = loadedFonts[fontPath];
                }

                return loadedFonts[fontPath];
            }

            try
            {
                var font = new Font(fontPath);
                loadedFonts[fontPath] = font;

                if (makeDefault)
                {
                    defaultFont = font;
                }

                return font;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load font from {fontPath}: {ex.Message}");
                return null;
            }
        }

        public static void Cleanup()
        {
            foreach (var font in loadedFonts.Values)
            {
                font.Dispose();
            }

            loadedFonts.Clear();
            defaultFont?.Dispose();
            defaultFont = null;
        }
    }
}