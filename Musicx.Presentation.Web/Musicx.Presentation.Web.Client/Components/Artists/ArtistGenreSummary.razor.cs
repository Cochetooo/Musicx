using Microsoft.AspNetCore.Components;
using Musicx.Contracts.Dto.Responses.Specifics.Artists;
using Musicx.Contracts.Enums;

namespace Musicx.Presentation.Web.Client.Components.Artists;

public partial class ArtistGenreSummary
{
    [Parameter] public IReadOnlyList<OutArtistGenreStat> PrimaryGenres { get; set; } = [];
    [Parameter] public IReadOnlyList<OutArtistGenreStat> Influences { get; set; } = [];
    [Parameter] public IReadOnlyList<OutArtistGenreStat> Descriptors { get; set; } = [];
    [Parameter] public IReadOnlyList<OutArtistGenreStat> Scenes { get; set; } = [];
    [Parameter] public IReadOnlyList<OutArtistGenreStat> Movements { get; set; } = [];
    [Parameter] public bool SimpleMode { get; set; }

    private bool _expanded;
    private bool _initialized;

    private IReadOnlyList<OutArtistGenreStat> AllPrimaryGenres =>
        PrimaryGenres.Where(x => x.Genre.Type == GenreType.Genre).ToList();

    private IReadOnlyList<OutArtistGenreStat> AllSubgenres =>
        PrimaryGenres.Where(x => x.Genre.Type is GenreType.Subgenre or GenreType.Fusion or GenreType.Localization).ToList();

    private IReadOnlyList<OutArtistGenreStat> AllScenesAndMovements =>
        Scenes.Concat(Movements).OrderByDescending(x => x.AlbumCount).ThenBy(x => x.Genre.CanonicalName).ToList();

    private IReadOnlyList<OutArtistGenreStat> PreviewPrimaryGenres => AllPrimaryGenres.Take(3).ToList();
    private IReadOnlyList<OutArtistGenreStat> PreviewSubgenres => AllSubgenres.Take(5).ToList();
    private IReadOnlyList<OutArtistGenreStat> PreviewInfluences => Influences.Take(4).ToList();
    private IReadOnlyList<OutArtistGenreStat> PreviewDescriptors => Descriptors.Take(4).ToList();
    private IReadOnlyList<OutArtistGenreStat> PreviewScenesAndMovements => AllScenesAndMovements.Take(4).ToList();

    private bool HasOverflow =>
        AllPrimaryGenres.Count > PreviewPrimaryGenres.Count
        || AllSubgenres.Count > PreviewSubgenres.Count
        || Influences.Count > PreviewInfluences.Count
        || Descriptors.Count > PreviewDescriptors.Count
        || AllScenesAndMovements.Count > PreviewScenesAndMovements.Count;

    protected override void OnParametersSet()
    {
        if (_initialized)
        {
            return;
        }

        _expanded = !SimpleMode;
        _initialized = true;
    }

    private void ToggleExpanded() => _expanded = !_expanded;

    private RenderFragment RenderSection(
        string title,
        string cardClass,
        IReadOnlyList<OutArtistGenreStat> stats,
        SectionKind kind) => builder =>
    {
        if (0 == stats.Count)
        {
            return;
        }

        builder.OpenElement(0, "section");
        builder.AddAttribute(1, "class", cardClass);

        builder.OpenElement(2, "div");
        builder.AddAttribute(3, "class", "artist-genre-section-header");

        builder.OpenElement(4, "span");
        builder.AddAttribute(5, "class", "artist-genre-section-title");
        builder.AddContent(6, title);
        builder.CloseElement();

        builder.OpenElement(7, "span");
        builder.AddAttribute(8, "class", "artist-genre-section-count");
        builder.AddContent(9, $"{stats.Count} entries");
        builder.CloseElement();

        builder.CloseElement();

        builder.OpenElement(10, "div");
        builder.AddAttribute(11, "class", "artist-genre-tags");

        var maxCount = Math.Max(1, stats.Max(x => x.AlbumCount));
        var seq = 12;

        foreach (var stat in stats)
        {
            builder.OpenElement(seq++, "a");
            builder.AddAttribute(seq++, "class", GetItemClass(kind, stat, maxCount));
            builder.AddAttribute(seq++, "href", $"/Genre/{stat.Genre.Id}");
            builder.AddAttribute(seq++, "title", $"{stat.Genre.CanonicalName} · {stat.AlbumCount} albums");

            builder.OpenElement(seq++, "span");
            builder.AddAttribute(seq++, "class", "artist-genre-name");
            builder.AddContent(seq++, stat.Genre.CanonicalName);
            builder.CloseElement();

            builder.OpenElement(seq++, "span");
            builder.AddAttribute(seq++, "class", "artist-genre-count");
            builder.AddContent(seq++, $"×{stat.AlbumCount}");
            builder.CloseElement();

            builder.CloseElement();
        }

        builder.CloseElement();
        builder.CloseElement();
    };

    private static string GetItemClass(SectionKind kind, OutArtistGenreStat stat, int maxCount)
    {
        var weightClass = GetWeightClass(kind, stat.AlbumCount, maxCount);
        return $"artist-genre-tag {weightClass} {GetKindClass(kind, stat)}";
    }

    private static string GetWeightClass(SectionKind kind, int count, int maxCount)
    {
        var ratio = maxCount <= 0 ? 0 : (double)count / maxCount;

        return kind switch
        {
            SectionKind.Primary => ratio switch
            {
                >= 0.95 => "is-weight-5",
                >= 0.75 => "is-weight-4",
                >= 0.55 => "is-weight-3",
                >= 0.35 => "is-weight-2",
                _ => "is-weight-1"
            },
            SectionKind.Influence => ratio switch
            {
                >= 0.85 => "is-weight-3",
                >= 0.55 => "is-weight-2",
                _ => "is-weight-1"
            },
            _ => ratio switch
            {
                >= 0.8 => "is-weight-4",
                >= 0.5 => "is-weight-3",
                >= 0.3 => "is-weight-2",
                _ => "is-weight-1"
            }
        };
    }

    private static string GetKindClass(SectionKind kind, OutArtistGenreStat stat) => kind switch
    {
        SectionKind.SceneMovement when stat.Genre.Type == GenreType.Movement => "is-movement",
        SectionKind.SceneMovement => "is-scene-item",
        _ => string.Empty
    };

    private enum SectionKind
    {
        Primary,
        Subgenre,
        Influence,
        SceneMovement,
        Descriptor
    }
}