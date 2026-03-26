using Musicx.Application.Shared.Helpers;

namespace Musicx.Presentation.Web.Client.Helpers;

public static class GenreBadgeStyleBuilder
{
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
}