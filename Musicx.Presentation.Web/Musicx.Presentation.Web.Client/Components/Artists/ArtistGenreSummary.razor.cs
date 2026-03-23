using System.Globalization;
using Microsoft.AspNetCore.Components;
using Musicx.Application.Shared.Helpers;
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

    private IReadOnlyList<ArtistGenreSection> Sections => new List<ArtistGenreSection>
    {
        CreateSection("Genres", "artist-genre-card is-primary", AllPrimaryGenres, 1, SectionKind.Primary),
        CreateSection("Subgenres", "artist-genre-card is-subgenre", AllSubgenres, 3, SectionKind.Subgenre),
        CreateSection("Influences", "artist-genre-card is-influence", Influences, 5, SectionKind.Influence),
        CreateSection("Scenes & Movements", "artist-genre-card is-scene", AllScenesAndMovements, 3, SectionKind.SceneMovement),
        CreateSection("Descriptors", "artist-genre-card is-descriptor", Descriptors, 15, SectionKind.Descriptor)
    }.Where(x => x.AllStats.Count > 0).ToList();

    private bool HasOverflow => Sections.Any(section => section.AllStats.Count > section.PreviewStats.Count);

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

    private static ArtistGenreSection CreateSection(
        string title,
        string cardClass,
        IReadOnlyList<OutArtistGenreStat> stats,
        int previewCount,
        SectionKind kind)
    {
        var orderedStats = stats
            .OrderByDescending(x => x.AlbumCount)
            .ThenBy(x => x.Genre.CanonicalName)
            .ToList();

        return new ArtistGenreSection(
            Title: title,
            CardClass: cardClass,
            Kind: kind,
            AllStats: orderedStats,
            PreviewStats: orderedStats.Take(previewCount).ToList());
    }
    
    private IReadOnlyList<OutArtistGenreStat> GetVisibleStats(ArtistGenreSection section)
        => _expanded ? section.AllStats : section.PreviewStats;
    
    private static int GetMaxCount(IReadOnlyList<OutArtistGenreStat> stats)
        => Math.Max(1, stats.Max(x => x.AlbumCount));
    
    private static string GetDisplayName(OutArtistGenreStat stat)
        => stat.Genre.ShortName ?? stat.Genre.CanonicalName;
    
    private static string GetItemTitle(OutArtistGenreStat stat)
        => $"{stat.Genre.CanonicalName} · {stat.AlbumCount} album{(stat.AlbumCount > 1 ? "s" : string.Empty)}";

    private static string GetItemClass(SectionKind kind, OutArtistGenreStat stat, int maxCount)
    {
        var weightClass = GetWeightClass(kind, stat.AlbumCount, maxCount);
        return $"artist-genre-tag {weightClass} {GetKindClass(kind, stat)}".Trim();
    }

    private static string GetWeightClass(SectionKind kind, int count, int maxCount)
    {
        var ratio = maxCount <= 0 ? 0 : (double)count / maxCount;

        return kind switch
        {
            SectionKind.Influence => ratio switch
            {
                >= 0.80 => "is-weight-3",
                >= 0.50 => "is-weight-2",
                _ => "is-weight-1"
            },
            _ => ratio switch
            {
                >= 0.85 => "is-weight-4",
                >= 0.62 => "is-weight-3",
                >= 0.35 => "is-weight-2",
                _ => "is-weight-1"
            }
        };
    }

    private static string GetKindClass(SectionKind kind, OutArtistGenreStat stat) => kind switch
    {
        SectionKind.SceneMovement when stat.Genre.Type == GenreType.Movement => "is-movement",
        SectionKind.SceneMovement => "is-scene-item",
        SectionKind.Influence => "is-influence-item",
        _ => string.Empty
    };
    
    private static string GetTagStyle(SectionKind kind, OutArtistGenreStat stat, int maxCount)
    {
        var baseColor = stat.Genre.Color ?? ColorHelper.DarkColor;
        var isLight = ColorHelper.IsColorLight(baseColor);
        var ratio = maxCount <= 0 ? 0 : (double)stat.AlbumCount / maxCount;
        var emphasis = Math.Clamp(0.35 + ratio * 0.45, 0.35, 0.80);
        var startAlpha = kind == SectionKind.Influence ? 0.25 + emphasis * 0.14 : 0.28 + emphasis * 0.18;
        var endAlpha = kind == SectionKind.Influence ? 0.17 + emphasis * 0.08 : 0.20 + emphasis * 0.12;
        var endColor = isLight
            ? ColorHelper.Interpolate(baseColor, "#FFFFFF", 0.72)
            : ColorHelper.Interpolate(baseColor, "#FFFFFF", 0.24);
        var borderAlpha = isLight ? 0.42 : 0.28;
        var glowAlpha = kind == SectionKind.Influence ? 0.10 : 0.16;
        var textColor = isLight ? ColorHelper.DarkColor : "#ffffff";
        var shadowColor = isLight ? "rgba(255,255,255,0.20)" : "rgba(0,0,0,0.24)";

        return string.Join(";", [
            $"--artist-genre-start:{ColorHelper.ToRgba(baseColor, startAlpha)}",
            $"--artist-genre-end:{ColorHelper.ToRgba(endColor, endAlpha)}",
            $"--artist-genre-border:{ColorHelper.ToRgba(baseColor, borderAlpha)}",
            $"--artist-genre-glow:{ColorHelper.ToRgba(baseColor, glowAlpha)}",
            $"--artist-genre-text:{textColor}",
            $"--artist-genre-shadow:{shadowColor}",
            $"--artist-genre-opacity:{GetOpacity(kind).ToString(CultureInfo.InvariantCulture)}"
        ]);
    }
    
    private static double GetOpacity(SectionKind kind) => kind switch
    {
        SectionKind.Influence => 0.84,
        SectionKind.Descriptor => 0.88,
        _ => 1
    };

    private sealed record ArtistGenreSection(
        string Title,
        string CardClass,
        SectionKind Kind,
        IReadOnlyList<OutArtistGenreStat> AllStats,
        IReadOnlyList<OutArtistGenreStat> PreviewStats);

    private enum SectionKind
    {
        Primary,
        Subgenre,
        Influence,
        SceneMovement,
        Descriptor
    }
}