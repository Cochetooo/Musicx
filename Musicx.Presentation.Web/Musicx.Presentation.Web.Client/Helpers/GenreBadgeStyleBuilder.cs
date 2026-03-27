using System.Text.RegularExpressions;
using Musicx.Application.Shared.Helpers;

namespace Musicx.Presentation.Web.Client.Helpers;

public static class GenreBadgeStyleBuilder
{
    private static readonly Regex HexColorRegex = new(@"#(?:[0-9a-fA-F]{3}|[0-9a-fA-F]{6})\b", RegexOptions.Compiled);
    
    public static string Build(
        string? color,
        double startAlpha = 0.56,
        double endAlpha = 0.36,
        double glowAlpha = 0.14,
        string? fontSize = null)
    {
        var baseColor = ColorHelper.NormalizeColor(color);
        var isLight = ColorHelper.IsColorLight(baseColor);
        var endColor = isLight
            ? ColorHelper.Interpolate(baseColor, "#FFFFFF", 0.72)
            : ColorHelper.Interpolate(baseColor, "#FFFFFF", 0.24);
        var borderAlpha = isLight ? 0.42 : 0.28;

        var styles = new List<string>
        {
            $"--artist-genre-start:{ColorHelper.ToRgba(baseColor, startAlpha)}",
            $"--artist-genre-end:{ColorHelper.ToRgba(endColor, endAlpha)}",
            $"--artist-genre-border:{ColorHelper.ToRgba(baseColor, borderAlpha)}",
            $"--artist-genre-glow:{ColorHelper.ToRgba(baseColor, glowAlpha)}",
            "--artist-genre-text:#ffffff",
            "--artist-genre-shadow:rgba(0,0,0,0.20)"
        };

        if (!string.IsNullOrWhiteSpace(fontSize))
        {
            styles.Add($"font-size:{fontSize}");
        }

        return string.Join(";", styles);
    }
    
    public static string BuildSimplified(string? colorOrGradient)
    {
        var colors = ExtractPalette(colorOrGradient);
        var primary = colors[0];
        var secondary = colors.Count > 1
            ? colors[1]
            : ColorHelper.Interpolate(primary, "#FFFFFF", 0.38);

        var isMultiColor = colors.Count > 1;
        var gradient = isMultiColor
            ? $"conic-gradient(from 210deg at 50% 50%, {ColorHelper.ToRgba(primary, 0.84)} 0deg, {ColorHelper.ToRgba(secondary, 0.76)} 160deg, {ColorHelper.ToRgba(primary, 0.80)} 360deg)"
            : $"radial-gradient(circle at 25% 20%, {ColorHelper.ToRgba(primary, 0.86)} 0%, {ColorHelper.ToRgba(secondary, 0.72)} 72%, {ColorHelper.ToRgba(primary, 0.64)} 100%)";

        var baseColor = ColorHelper.NormalizeColor(primary);
        var borderColor = ColorHelper.ToRgba(baseColor, ColorHelper.IsColorLight(baseColor) ? 0.38 : 0.24);

        return string.Join(";", new[]
        {
            Build(baseColor),
            $"--simplified-genre-gradient:{gradient}",
            $"--artist-genre-border:{borderColor}"
        });
    }

    private static List<string> ExtractPalette(string? colorOrGradient)
    {
        var fallback = new List<string> { ColorHelper.DarkColor };

        if (string.IsNullOrWhiteSpace(colorOrGradient))
        {
            return fallback;
        }

        var matches = HexColorRegex.Matches(colorOrGradient);
        if (matches.Count == 0)
        {
            return new List<string> { ColorHelper.NormalizeColor(colorOrGradient) };
        }

        return matches
            .Select(m => ColorHelper.NormalizeColor(m.Value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(4)
            .ToList();
    }
}