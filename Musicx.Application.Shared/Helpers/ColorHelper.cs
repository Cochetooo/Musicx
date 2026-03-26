using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Musicx.Application.Shared.Helpers;

public static partial class ColorHelper
{
    public const string DarkColor = "#2b3333";
    private static readonly Regex RgbRegex = new(@"rgba?\(([^)]*)\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    public static bool IsColorLight(string? color)
    {
        if (!TryParseRgb(color, out var r, out var g, out var b))
        {
            return false;
        }
        double brightness = (0.299 * r + 0.587 * g + 0.114 * b);
        return brightness > 186;
    }

    public static string NormalizeColor(string? color)
    {
        if (!TryParseRgb(color, out var r, out var g, out var b))
        {
            return DarkColor;
        }
        
        return $"#{r:X2}{g:X2}{b:X2}";
    }

    public static string LightenColor(string hexColor, double amount)
    {
        var normalized = NormalizeColor(hexColor);

        var r = int.Parse(normalized.Substring(1, 2), NumberStyles.HexNumber);
        var g = int.Parse(normalized.Substring(3, 2), NumberStyles.HexNumber);
        var b = int.Parse(normalized.Substring(5, 2), NumberStyles.HexNumber);

        r = (int)Math.Min(255, r + 255 * amount);
        g = (int)Math.Min(255, g + 255 * amount);
        b = (int)Math.Min(255, b + 255 * amount);

        return $"#{r:X2}{g:X2}{b:X2}";
    }
    
    public static string ToRgba(string? color, double alpha)
    {
        var alphaInvariant = alpha.ToString(CultureInfo.InvariantCulture);

        if (!TryParseRgb(color, out var r, out var g, out var b))
        {
            return $"rgba(0,0,0,{alphaInvariant})";
        }

        return $"rgba({r},{g},{b},{alphaInvariant})";
    }

    public static string Interpolate(string color1, string color2, double factor)
    {
        var normalizedColor1 = NormalizeColor(color1);
        var normalizedColor2 = NormalizeColor(color2);

        int c1 = Convert.ToInt32(normalizedColor1[1..], 16);
        int c2 = Convert.ToInt32(normalizedColor2[1..], 16);

        int r1 = (c1 >> 16) & 0xFF;
        int g1 = (c1 >> 8) & 0xFF;
        int b1 = c1 & 0xFF;
        
        int r2 = (c2 >> 16) & 0xFF;
        int g2 = (c2 >> 8) & 0xFF;
        int b2 = c2 & 0xFF;

        int r = (int)Math.Round(r1 + factor * (r2 - r1));
        int g = (int)Math.Round(g1 + factor * (g2 - g1));
        int b = (int)Math.Round(b1 + factor * (b2 - b1));

        return $"#{r:X2}{g:X2}{b:X2}";
    }
    
    private static bool TryParseRgb(string? color, out int r, out int g, out int b)
    {
        r = g = b = 0;

        if (string.IsNullOrWhiteSpace(color))
        {
            return false;
        }

        var input = color.Trim();

        var rgbMatch = RgbRegex.Match(input);
        if (rgbMatch.Success)
        {
            var chunks = rgbMatch.Groups[1].Value
                .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            if (chunks.Length >= 3
                && TryParseRgbChannel(chunks[0], out r)
                && TryParseRgbChannel(chunks[1], out g)
                && TryParseRgbChannel(chunks[2], out b))
            {
                return true;
            }
        }

        var hex = input.TrimStart('#');
        if (hex.Length == 3)
        {
            hex = string.Concat(hex.Select(c => $"{c}{c}"));
        }

        if (hex.Length == 6 && int.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var _))
        {
            r = Convert.ToInt32(hex.Substring(0, 2), 16);
            g = Convert.ToInt32(hex.Substring(2, 2), 16);
            b = Convert.ToInt32(hex.Substring(4, 2), 16);
            return true;
        }

        var namedColor = Color.FromName(input);
        if (namedColor.IsKnownColor || namedColor.IsNamedColor)
        {
            r = namedColor.R;
            g = namedColor.G;
            b = namedColor.B;
            return true;
        }

        return false;
    }

    private static bool TryParseRgbChannel(string input, out int value)
    {
        value = 0;

        if (double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
        {
            value = (int)Math.Clamp(Math.Round(parsed), 0, 255);
            return true;
        }

        return false;
    }
}