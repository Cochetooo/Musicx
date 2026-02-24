using MudBlazor;
using Musicx.Contracts.Dto.Responses.Genre;
using Musicx.Contracts.Enums;

namespace Musicx.Presentation.Web.Client.Helpers;

public static class UiGenreHelper
{
    public static string GetGenreIcon(OutGenre genre)
        => genre.Type switch
        {
            GenreType.Subgenre => Icons.Material.Filled.Label,
            GenreType.Scene => Icons.Material.Filled.Language,
            GenreType.Movement => Icons.Material.Filled.Album,
            GenreType.Descriptor => Icons.Material.Filled.TheaterComedy,
            GenreType.Fusion => Icons.Material.Filled.Diversity2,
            GenreType.Genre => Icons.Material.Filled.MusicNote,
            GenreType.Localization => Icons.Material.Filled.Category,
            _ => ""
        };
}