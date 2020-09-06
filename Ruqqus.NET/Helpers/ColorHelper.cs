using System;
using System.Drawing;

namespace Ruqqus.Helpers
{
    /// <summary>
    /// Helper class for converting colors, as the System.Drawing.ColorTranslator is not present in netstandard.
    /// </summary>
    internal static class ColorHelper
    {
        /// <summary>
        /// Converts a color representing as an HTML string into a <see cref="Color"/>.
        /// </summary>
        /// <param name="htmlColor">A valid HTML color string, in #RRGGBB or #RGB format.</param>
        /// <returns>The string as a <see cref="Color"/> instance.</returns>
        public static Color FromHtml(string htmlColor)
        {
            var c = Color.Empty;
            if (string.IsNullOrEmpty(htmlColor))
                return c;

            // Color is not a valid HTML color in #RRGGBB or #RGB format
            if ((htmlColor[0] != '#') || ((htmlColor.Length != 7) && (htmlColor.Length != 4)))
                return c;

            if (htmlColor.Length == 7)
            {
                c = Color.FromArgb(Convert.ToInt32(htmlColor.Substring(1, 2), 16),
                    Convert.ToInt32(htmlColor.Substring(3, 2), 16), Convert.ToInt32(htmlColor.Substring(5, 2), 16));
            }
            else
            {
                var r = char.ToString(htmlColor[1]);
                var g = char.ToString(htmlColor[2]);
                var b = char.ToString(htmlColor[3]);

                c = Color.FromArgb(Convert.ToInt32(r + r, 16), Convert.ToInt32(g + g, 16), Convert.ToInt32(b + b, 16));
            }

            return c;
        }
    }
}