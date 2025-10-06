using System.Drawing;
using System.Globalization;

namespace Musicx.Application.Shared.Utilities;

public static class ColorHelper
{
    public const string DarkColor = "#2b3333";
    
    public static bool IsColorLight(string hexColor)
    {
        if (hexColor.StartsWith("#"))
        {
            hexColor = hexColor[1..];
        }

        if (hexColor.Length == 3)
        {
            hexColor = string.Concat(hexColor.Select(c => $"{c}{c}"));
        }

        if (hexColor.Length != 6)
        {
            return false;
        }
        
        var r = Convert.ToInt32(hexColor.Substring(0, 2), 16);
        var g = Convert.ToInt32(hexColor.Substring(2, 2), 16);
        var b = Convert.ToInt32(hexColor.Substring(4, 2), 16);
        
        double brightness = (0.299 * r + 0.587 * g + 0.114 * b);
        return brightness > 186;
    }

    public static string LightenColor(string hexColor, double amount)
    {
        if (hexColor.StartsWith("#"))
        {
            hexColor = hexColor[1..];
        }
        
        if (hexColor.Length == 3)
        {
            hexColor = string.Concat(hexColor.Select(c => $"{c}{c}"));
        }

        if (6 != hexColor.Length)
        {
            return "#" + hexColor;
        }
        
        var r = int.Parse(hexColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        var g = int.Parse(hexColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        var b = int.Parse(hexColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

        r = (int)Math.Min(255, r + 255 * amount);
        g = (int)Math.Min(255, g + 255 * amount);
        b = (int)Math.Min(255, b + 255 * amount);

        return $"#{r:X2}{g:X2}{b:X2}";
    }
    
    public static string ToRgba(string hexColor, double alpha)
    {
        var alphaInvariant = alpha.ToString(CultureInfo.InvariantCulture);
        
        if (string.IsNullOrWhiteSpace(hexColor))
            return $"rgba(0,0,0,{alphaInvariant})";

        hexColor = hexColor.TrimStart('#');

        if (hexColor.Length == 6)
        {
            var r = Convert.ToInt32(hexColor.Substring(0, 2), 16);
            var g = Convert.ToInt32(hexColor.Substring(2, 2), 16);
            var b = Convert.ToInt32(hexColor.Substring(4, 2), 16);
            return $"rgba({r},{g},{b},{alphaInvariant})";
        }

        return $"rgba(0,0,0,{alphaInvariant})";
    }

    public static string Interpolate(string color1, string color2, double factor)
    {
        int c1 = Convert.ToInt32(color1[1..], 16);
        int c2 = Convert.ToInt32(color2[1..], 16);

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
}