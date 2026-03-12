using Musicx.Application.Api.Interfaces.Caching;
using Musicx.Application.Api.Interfaces.DataViews;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.Album;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.Artist;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.Genre;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.User;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Helpers;
using Musicx.Contracts.Dto.Responses.Specifics.Artists;
using Musicx.Contracts.Dto.Responses.Specifics.Genres;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Contracts.Dto.Responses.Specifics.Ratings;
using Musicx.Infrastructure.API.Persistence.Specifications.Album;
using Musicx.Infrastructure.API.Persistence.Specifications.Genre;

namespace Musicx.Infrastructure.API.DataViews;

public sealed class GenreDataViewBuilder(
    IGenreRepository genreRepository,
    IArtistRepository artistRepository,
    IAlbumRepository albumRepository,
    IUserAlbumAttrsRepository userAlbumAttrsRepository,
    IDataViewCacheProvider cache,
    IDataViewCacheKeyFactory cacheKeyFactory) : IGenreDataViewBuilder
{
    public async Task<OutGenreDataView?> BuildAsync(GenreDataViewQuery query, CancellationToken cancellationToken = default)
    {
        var cacheKey = cacheKeyFactory.BuildGenreDataViewKey(query.GenreId, query.UserId);
        var cached = await cache.GetAsync<OutGenreDataView>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            return cached;
        }

        var genre = await genreRepository.FindOneByIdAsync(query.GenreId, new GenreJoinSpecification
        {
            IncludeParents = true,
            IncludeChildren = true,
            IncludeAliases = true,
        });

        if (genre is null)
        {
            return null;
        }

        var artistsTask = artistRepository.FindByGenreIdAsync(query.GenreId, new PagingOptions(Skip: 0, Take: 10));
        var topAlbumsTask = albumRepository.FindByGenreIdAsync(
            genreId: query.GenreId,
            genreOptions: GenreOptions.PrimaryGenre,
            pagingOptions: new PagingOptions(Take: 10, Skip: 0),
            orderSpec: new AlbumOrderSpecification { RatingCount = -1, RatingAverage = -2, Name = 3 },
            joinSpec: new AlbumJoinSpecification
            {
                IncludeArtist = true,
                IncludePrimaryGenres = true,
                IncludeStats = true
            });
        var yearlyRatingsTask = query.UserId.HasValue
            ? userAlbumAttrsRepository.GetUserYearlyRatingsAsync(bucketSize: 5, genreId: query.GenreId)
            : Task.FromResult<IReadOnlyList<OutUserYearlyRating>>([]);

        await Task.WhenAll(
            artistsTask,
            topAlbumsTask,
            yearlyRatingsTask
        );

        var artists = artistsTask.Result;
        var topAlbums = topAlbumsTask.Result;
        var yearlyRatings = yearlyRatingsTask.Result;

        var artistIds = artists.Select(x => x.Id).ToArray();
        var albumsByArtist = await albumRepository.FindByArtistIdsAsync(
            artistIds,
            new AlbumJoinSpecification { IncludeStats = true },
            new AlbumOrderSpecification { OriginalReleaseDate = -1 });

        var topArtists = artists
            .Select(artist =>
            {
                albumsByArtist.TryGetValue(artist.Id, out var discography);
                var summary = RatingHelper.CalculateArtistRatingSummary(discography ?? []);
                return new OutArtistAlbumSummary
                {
                    ArtistId = artist.Id,
                    Artist = artist,
                    Rating = summary.Rating,
                    RatingsCount = summary.Count
                };
            })
            .OrderByDescending(x => x.RatingsCount)
            .ThenByDescending(x => x.Rating ?? 0)
            .ToList();

        var result = new OutGenreDataView
        {
            Genre = genre,
            Artists = new OutGenericList<Musicx.Contracts.Dto.Responses.OutArtist>
            {
                Items = artists.ToList(),
                Total = await artistRepository.GetCountByGenreIdAsync(query.GenreId)
            },
            TopAlbums = new OutAlbumList
            {
                Items = topAlbums,
                Total = topAlbums.Count,
                AverageRating = RatingHelper.CalculateArtistRating(topAlbums)
            },
            TopArtists = topArtists,
            YearlyRatings = yearlyRatings,
            AlbumsAverageRating = yearlyRatings.Count == 0 ? null : yearlyRatings.Select(r => r.AverageRating).Average()
        };

        await cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5), cancellationToken);
        return result;
    }
}