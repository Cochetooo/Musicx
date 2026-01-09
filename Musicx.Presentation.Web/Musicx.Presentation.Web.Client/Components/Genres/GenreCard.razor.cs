using Microsoft.AspNetCore.Components;
using Musicx.Application.Shared.Helpers;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Presentation.Web.Client.Components.Genres;

public partial class GenreCard : ComponentBase
{
    [Parameter, EditorRequired] public OutGenre Genre { get; set; } = null!;
    [Parameter] public int Depth { get; set; } = 0;
    
    private bool IsExpanded { get; set; }
    
    private void ToggleExpand() => IsExpanded = !IsExpanded;

    private string GetClasses() =>
        IsExpanded
            ? "genre-card expanded"
            : "genre-card";
    
    private string GetStyle()
    {
        var baseColor = Genre.Color ?? ColorHelper.DarkColor;
        var lighterColor = ColorHelper.LightenColor(baseColor, Depth * 0.1);
        var color = ColorHelper.IsColorLight(lighterColor) ? ColorHelper.DarkColor : "white";
        return $"background-color: {lighterColor}; color: {color};";
    }
}