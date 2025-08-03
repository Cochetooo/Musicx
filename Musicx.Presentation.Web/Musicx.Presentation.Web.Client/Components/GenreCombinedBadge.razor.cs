using System.Drawing;
using Microsoft.AspNetCore.Components;
using Musicx.Application.Shared.Utilities;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Presentation.Web.Client.Components;

public partial class GenreCombinedBadge
{
    [Parameter, EditorRequired] public List<OutGenre> Genres { get; set; } = null!;

    private string GetGradientStyle(List<OutGenre> genres)
    {
        if (0 == genres.Count)
        {
            // Should NEVER happen
            return $"background-color: {ColorHelper.DarkColor}; color: white;";
        }

        var gradient = "linear-gradient(to right, ";
        var stops = genres.Select(g => g.Color ?? ColorHelper.DarkColor);
        gradient += string.Join(", ", stops) + ")";

        var lightText = genres.Count(g => !ColorHelper.IsColorLight(g.Color ?? ColorHelper.DarkColor)) > genres.Count / 2;
        var textColor = lightText ? "white" : ColorHelper.DarkColor;

        return $"background: {gradient}; color: {textColor}";
    }
}