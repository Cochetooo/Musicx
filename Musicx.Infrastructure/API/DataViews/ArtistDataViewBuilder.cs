using System.Text.Json;
using Musicx.Application.Api.Interfaces.Caching;
using Musicx.Application.Api.Interfaces.DataViews;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.Album;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.Artist;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.Genre;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.User;
using Musicx.Application.API.Persistence.Queries;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Helpers;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Artists;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Infrastructure.API.Persistence.Specifications.Album;
using Musicx.Infrastructure.API.Persistence.Specifications.Artist;
using AlbumJoinSpecification = Musicx.Infrastructure.API.Persistence.Specifications.Album.AlbumJoinSpecification;

namespace Musicx.Infrastructure.API.DataViews;

public sealed class ArtistDataViewBuilder(
    IArtistRepository artistRepository,
    IGenreRepository genreRepository,
    IAlbumRepository albumRepository,
    IUserAlbumAttrsRepository userAlbumAttrsRepository,
    IUserArtistAttrsRepository userArtistAttrsRepository,
    IDataViewCacheProvider cache,
    IDataViewCacheKeyFactory cacheKeyFactory) : IArtistDataViewBuilder
{
    public async Task<OutArtistDataView?> BuildAsync(ArtistDataViewQuery query, CancellationToken cancellationToken = default)
    {
        var cacheKey = cacheKeyFactory.BuildArtistDataViewKey(query.ArtistId, query.UserId);
        var cached = await cache.GetAsync<OutArtistDataView>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            return cached;
        }

        var artistTask = artistRepository.FindOneByIdAsync(query.ArtistId, new ArtistJoinSpecification
        {
            IncludeStats = true
        });
        
        var albumsTask = albumRepository.FindByArtistIdAsync(
            query.ArtistId,
            new AlbumJoinSpecification
            {
                IncludePrimaryGenres = true,
                IncludeInfluenceGenres = true,
                IncludeStats = true
            },
            new AlbumOrderSpecification
            {
                OriginalReleaseDate = 1,
                Name = 2
            }
        );
        
        Task<List<OutUserAlbumAttribute>> userAlbumAttrsTask = Task.FromResult<List<OutUserAlbumAttribute>>([]);
        Task<long> userAlbumAttrsCountTask = Task.FromResult(0L);
        Task<bool> isFollowingTask = Task.FromResult(false);
        
        if (query.UserId is not null)
        {
            userAlbumAttrsTask = userAlbumAttrsRepository.FindAsync(
                new UserAlbumAttrFindQuery
                {
                    UserId = query.UserId,
                    ArtistId = query.ArtistId
                },
                pagingOptions: new PagingOptions(100_000, 0)
            ).ContinueWith(t => t.Result.ToList(), cancellationToken);

            userAlbumAttrsCountTask = userAlbumAttrsRepository.CountByUserIdAsync(
                query.UserId.Value,
                null,
                query.ArtistId
            );

            isFollowingTask = userArtistAttrsRepository
                .FindOneAsync(query.UserId.Value, query.ArtistId)
                .ContinueWith(t => t.Result is { Follow: true }, cancellationToken);
        }
        
        var followersCountTask = userArtistAttrsRepository.CountFollowersByArtistAsync(query.ArtistId);

        await Task.WhenAll(
            artistTask,
            albumsTask,
            userAlbumAttrsTask,
            userAlbumAttrsCountTask,
            followersCountTask,
            isFollowingTask
        );
        
        var artist = await artistTask;
        if (artist is null)
        {
            return null;
        }
        
        var albums = await albumsTask;

        if (artist.Stats is null)
        {
            artist.Stats = RatingHelper.CalculateArtistRatingSummary(albums);
        }
        
        var primaryGenresTask = BuildGenreStatsAsync(artist.CalculatedGenreCounts, cancellationToken);
        var influencesTask = BuildGenreStatsAsync(artist.CalculatedInfluenceCounts, cancellationToken);
        var descriptorsTask = BuildGenreStatsAsync(artist.CalculatedDescriptorCounts, cancellationToken);
        var scenesTask = BuildGenreStatsAsync(artist.CalculatedSceneCounts, cancellationToken);
        var movementsTask = BuildGenreStatsAsync(artist.CalculatedMovementCounts, cancellationToken);
        
        await Task.WhenAll(primaryGenresTask, influencesTask, descriptorsTask, scenesTask, movementsTask);

        var primaryGenres = await primaryGenresTask;
        var influences = await influencesTask;
        
        var primaryGenreIds = primaryGenres.Select(g => g.Genre.Id).ToHashSet();

        influences = influences
            .Where(g => !primaryGenreIds.Contains(g.Genre.Id))
            .ToList();
        
        OutGenericList<OutUserAlbumAttribute>? userAlbumAttrsList = null;

        if (query.UserId is not null)
        {
            userAlbumAttrsList = new OutGenericList<OutUserAlbumAttribute>
            {
                Items = await userAlbumAttrsTask,
                Total = await userAlbumAttrsCountTask
            };
        }

        var result = new OutArtistDataView
        {
            Artist = artist,
            Albums = new OutAlbumList
            {
                AverageRating = RatingHelper.CalculateArtistRating(albums),
                Items = albums,
                Total = albums.Count
            },
            PrimaryGenres = primaryGenres,
            Influences = influences,
            Descriptors = await descriptorsTask,
            Scenes = await scenesTask,
            Movements = await movementsTask,
            UserAttributes = userAlbumAttrsList,
            FollowersCount = await followersCountTask,
            IsCurrentUserFollowing = await isFollowingTask
        };
        
        await cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5), cancellationToken);

        return result;
    }
    
    private async Task<IReadOnlyList<OutArtistGenreStat>> BuildGenreStatsAsync(string? serializedStats, CancellationToken cancellationToken)
    {
        var parsedStats = ParseGenreStats(serializedStats);
        if (0 == parsedStats.Count)
        {
            return [];
        }

        var genres = await genreRepository.FindInAsync(parsedStats.Select(x => x.GenreId));
        cancellationToken.ThrowIfCancellationRequested();

        var genreById = genres.ToDictionary(x => x.Id);

        return parsedStats
            .Where(x => genreById.ContainsKey(x.GenreId))
            .Select(x => new OutArtistGenreStat
            {
                Genre = genreById[x.GenreId],
                AlbumCount = x.Count
            })
            .ToList();
    }
    
    private static List<ArtistGenreCountRow> ParseGenreStats(string? serializedStats)
    {
        if (string.IsNullOrWhiteSpace(serializedStats))
        {
            return [];
        }

        try
        {
            return JsonSerializer.Deserialize<List<ArtistGenreCountRow>>(serializedStats, JsonHelper.OptionsDefault) ?? [];
        }
        catch (JsonException)
        {
            return [];
        }
    }

    private sealed class ArtistGenreCountRow
    {
        public long GenreId { get; set; }
        public int Count { get; set; }
    }
}