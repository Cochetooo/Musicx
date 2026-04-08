using System.Text.Json;
using Musicx.Application.API.Persistence.Queries;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence.Filtering;
using Musicx.Contracts.Dto.Requests.Artist;
using Musicx.Contracts.Dto.Requests.Genre;
using Musicx.Contracts.Dto.Responses.Artist;
using Musicx.Contracts.Dto.Responses.Genre;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Contracts.Enums;
using Musicx.Infrastructure.API.Persistence.Specifications.Artist;

namespace Musicx.Presentation.Web.Client.Pages.Charts;

public partial class ArtistChart
{
    private OutGenericList<OutArtist> _artists = new();
    private ArtistFindQuery _query = new();
    private string _queryText = string.Empty;
    private bool _advanced;
    private bool _isLoading;

    private short _popularityWeight = 5;
    private ChartType _chartType = ChartType.Top;
    private ArtistDiscriminator _discriminator = ArtistDiscriminator.Artist;
    
    private List<OutGenre> _genres = [];
    private List<OutGenre> _mainGenres = [];
    private List<OutGenre> _influenceCandidates = [];

    protected override async Task OnInitializedAsync()
    {
        await LoadGenres();
        await LoadArtists();
    }

    private async Task LoadGenres()
    {
        var genres = await Api.FindAsync<InGenre, OutGenre>(pagingOptions: new PagingOptions(10_000, 0));
        _genres = genres.Items.ToList();
        _mainGenres = _genres.Where(g => g.Type == GenreType.Genre).Take(20).ToList();
        _influenceCandidates = _genres.Where(g => g.Type != GenreType.Descriptor).Take(24).ToList();
    }

    private async Task LoadArtists()
    {
        _isLoading = true;
        _query.RawSearch = string.IsNullOrWhiteSpace(_queryText) ? null : new TextFilter(_queryText);
        _query.Discriminator = _discriminator == ArtistDiscriminator.Artist ? null : _discriminator;
        _query.PopularityWeight = _popularityWeight;
        _query.ChartType = _chartType;
        _artists = await Api.FindAsync<InArtist, OutArtist>(
            query: _query, 
            joins: new ArtistJoinSpecification
            {
                IncludeStats = true
            },
            pagingOptions: new PagingOptions(100, 0)
        );
        _isLoading = false;
    }

    private IReadOnlyList<OutGenre> GetDisplayGenres(OutArtist artist)
    {
        if (string.IsNullOrWhiteSpace(artist.CalculatedGenreCounts)) return [];
        try
        {
            using var doc = JsonDocument.Parse(artist.CalculatedGenreCounts);
            var ids = doc.RootElement.EnumerateArray().Select(x => x.GetProperty("genreId").GetInt64()).ToHashSet();
            return _genres.Where(g => ids.Contains(g.Id)).ToList();
        }
        catch { return []; }
    }

    private bool IsInfluenceSelected(long id) => _query.InfluenceGenreIds?.Contains(id) == true;

    private void ToggleInfluence(long id)
    {
        var current = _query.InfluenceGenreIds?.ToHashSet() ?? [];
        if (!current.Add(id)) current.Remove(id);
        _query.InfluenceGenreIds = current.ToArray();
    }
}