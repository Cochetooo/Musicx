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
}