using Microsoft.AspNetCore.Components;
using MudBlazor;
using Musicx.Application.Shared.Enums;
using Musicx.Contracts.Dto.Requests.Genre;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Genre;
using Musicx.Contracts.Dto.Responses.User;
using Musicx.Infrastructure.API.Persistence.Mappers;

namespace Musicx.Presentation.Web.Client.Modals.Voting;

public partial class AlbumSongGenreVoteModal
{
    [Parameter] public EventCallback OnSave { get; set; }

    private sealed class SongVoteRow
    {
        public OutSong Song { get; init; } = null!;
        public InUserSongAttribute Attribute { get; init; } = new();
        public List<OutGenre> PrimaryGenres { get; init; } = [];
        public List<OutGenre> InfluenceGenres { get; init; } = [];
        public OutGenre? SelectedPrimaryGenre { get; set; }
        public OutGenre? SelectedInfluenceGenre { get; set; }
    }

    private sealed record RatingFactor(
        string Label,
        Func<InUserSongAttribute, short?> Getter,
        Action<InUserSongAttribute, short?> Setter);

    private readonly List<SongVoteRow> _songRows = [];
    private readonly List<RatingFactor> _factors =
    [
        new("Production", x => x.ProductionRating, (x, v) => x.ProductionRating = v),
        new("Lyrics", x => x.LyricsRating, (x, v) => x.LyricsRating = v),
        new("Instrumentation", x => x.InstrumentationRating, (x, v) => x.InstrumentationRating = v),
        new("Vocals", x => x.VocalsRating, (x, v) => x.VocalsRating = v),
        new("Atmosphere", x => x.AtmosphereRating, (x, v) => x.AtmosphereRating = v),
        new("Originality", x => x.OriginalityRating, (x, v) => x.OriginalityRating = v)
    ];

    private MudDialog _modalRef = null!;
    private List<OutGenre> _genres = [];
    private bool _autoCompute;

    protected override async Task OnInitializedAsync()
    {
        _genres = (await Api.FindAsync<InGenre, OutGenre>(pagingOptions: new PagingOptions(100_000, 0))).Items
            .Where(x => x is { IsVisible: true, IsTaggable: true })
            .ToList();
    }

    public async Task Show(IReadOnlyList<OutSong> songs, IReadOnlyList<OutUserSongAttribute> currentUserSongAttributes)
    {
        _songRows.Clear();

        _autoCompute = UserClientContext.CurrentUser?.PrefAutoComputeAdvancedRatings ?? false;
        var userId = UserClientContext.CurrentUser?.Id ?? 0;
        var attrsBySongId = currentUserSongAttributes.ToDictionary(x => x.Song.Id, x => x);

        foreach (var song in songs)
        {
            attrsBySongId.TryGetValue(song.Id, out var currentAttr);

            var attribute = currentAttr?.ToRaw() ?? new InUserSongAttribute
            {
                UserId = userId,
                SongId = song.Id
            };

            _songRows.Add(new SongVoteRow
            {
                Song = song,
                Attribute = attribute,
                PrimaryGenres = song.PrimaryGenres?.ToList() ?? [],
                InfluenceGenres = song.InfluenceGenres?.ToList() ?? []
            });
        }

        await _modalRef.ShowAsync();
    }

    private Task<IEnumerable<OutGenre?>> SearchGenres(string? value, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Task.FromResult<IEnumerable<OutGenre?>>([]);
        }

        var result = _genres.Where(g =>
            g.CanonicalName.Contains(value, StringComparison.InvariantCultureIgnoreCase)
            || (g.ShortName?.Contains(value, StringComparison.InvariantCultureIgnoreCase) ?? false));

        return Task.FromResult(result.Cast<OutGenre?>());
    }

    private static string GenreName(OutGenre genre) => genre.ShortName ?? genre.CanonicalName;

    private static string GetSongTitle(OutSong song)
        => $"{song.DiscNumber ?? 1}.{song.TrackNumber ?? 0} · {song.Title}";

    private void AddPrimaryGenre(SongVoteRow row)
    {
        if (row.SelectedPrimaryGenre is null || row.PrimaryGenres.Any(x => x.Id == row.SelectedPrimaryGenre.Id))
        {
            return;
        }

        row.PrimaryGenres.Add(row.SelectedPrimaryGenre);
        row.SelectedPrimaryGenre = null;
    }

    private void AddInfluenceGenre(SongVoteRow row)
    {
        if (row.SelectedInfluenceGenre is null || row.InfluenceGenres.Any(x => x.Id == row.SelectedInfluenceGenre.Id))
        {
            return;
        }

        row.InfluenceGenres.Add(row.SelectedInfluenceGenre);
        row.SelectedInfluenceGenre = null;
    }

    private void OnSongRatingChanged(SongVoteRow row, int? rating) => row.Attribute.Rating = (short?)rating;

    private void OnFactorChanged(SongVoteRow row, RatingFactor factor, short? value)
    {
        factor.Setter(row.Attribute, value);

        if (_autoCompute && HasAllFactors(row.Attribute))
        {
            ComputeFinal(row);
        }
    }

    private void OnAutoComputeChanged(bool value)
    {
        _autoCompute = value;

        if (!_autoCompute)
        {
            return;
        }

        foreach (var row in _songRows.Where(row => HasAllFactors(row.Attribute)))
        {
            ComputeFinal(row);
        }
    }

    private bool CanCompute(InUserSongAttribute attribute) => GetFactorValues(attribute).Count > 0;

    private bool HasAllFactors(InUserSongAttribute attribute)
        => _factors.All(f => f.Getter(attribute).HasValue);

    private List<short> GetFactorValues(InUserSongAttribute attribute)
        => _factors.Select(f => f.Getter(attribute)).Where(v => v.HasValue).Select(v => v!.Value).ToList();

    private void ComputeFinal(SongVoteRow row)
    {
        var values = GetFactorValues(row.Attribute);
        if (values.Count == 0)
        {
            return;
        }

        row.Attribute.Rating = (short)Math.Round(values.Select(v => (int)v).Average());
    }

    private async Task Save()
    {
        foreach (var row in _songRows)
        {
            if (row.Attribute.UserId > 0)
            {
                await Api.SaveAsync(row.Attribute);
            }

            var songRaw = row.Song.ToRaw();
            songRaw.PrimaryGenreIds = row.PrimaryGenres.Select(g => g.Id).ToList();
            songRaw.InfluenceGenreIds = row.InfluenceGenres.Select(g => g.Id).ToList();
            await Api.SaveAsync(songRaw);
        }

        Snackbar.Add("Songs updated.", Severity.Success);
        await OnSave.InvokeAsync();
        await Hide();
    }

    private async Task Hide() => await _modalRef.CloseAsync();
}