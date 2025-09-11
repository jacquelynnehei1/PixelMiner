using SFML.Graphics;

namespace pixel_miner.Utils.Extensions
{
    public static class ColorExtensions
    {
        public static Color Lerp(this Color a, Color b, float t)
        {
            t = Math.Max(0f, Math.Min(1f, t));

            return new Color(
                (byte)(a.R + (b.R - a.R) * t),
                (byte)(a.G + (b.G - a.G) * t),
                (byte)(a.B + (b.B - a.B) * t),
                (byte)(a.A + (b.A - a.A) * t)
            );
        }
    }
}