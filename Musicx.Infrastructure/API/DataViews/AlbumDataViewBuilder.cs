using Musicx.Application.Api.Interfaces.Caching;
using Musicx.Application.Api.Interfaces.DataViews;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.Album;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.Song;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.User;
using Musicx.Application.API.Persistence.Queries;
using Musicx.Application.Shared.Enums;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Albums;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Infrastructure.API.Persistence.Specifications.Album;
using Musicx.Infrastructure.API.Persistence.Specifications.Song;
using Musicx.Infrastructure.API.Persistence.Specifications.User;

namespace Musicx.Infrastructure.API.DataViews;

/// <summary>
/// Builds a consolidated Album DataView from multiple repositories.
/// </summary>
/// <since>0.7.4</since>
public sealed class AlbumDataViewBuilder(
    IAlbumRepository albumRepository,
    ISongRepository songRepository,
    IUserAlbumAttrsRepository userAlbumAttrsRepository,
    IDataViewCacheProvider cache,
    IDataViewCacheKeyFactory cacheKeyFactory) : IAlbumDataViewBuilder
{
    public async Task<OutAlbumDataView?> BuildAsync(AlbumDataViewQuery query, CancellationToken cancellationToken = default)
    {
        var cacheKey = cacheKeyFactory.BuildAlbumDataViewKey(query.AlbumId, query.UserId);
        var cached = await cache.GetAsync<OutAlbumDataView>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            return cached;
        }

        var album = await albumRepository.FindOneByIdAsync(query.AlbumId, new AlbumJoinSpecification
        {
            IncludeArtist = true,
            IncludePrimaryGenres = true,
            IncludeInfluenceGenres = true,
            IncludeReleases = true,
            IncludeStats = true
        });

        if (album is null)
        {
            return null;
        }

        var songsTask = songRepository.FindByAlbumIdAsync(album.Id, orderSpec: new SongOrderSpecification
        {
            TrackNumber = 1,
            Title = 2
        });

        Task<List<OutAlbum>> artistAlbumsTask = Task.FromResult<List<OutAlbum>>([]);
        if (album.Artist is not null)
        {
            artistAlbumsTask = albumRepository.FindByArtistIdAsync(album.Artist.Id, orderSpecification: new AlbumOrderSpecification
            {
                OriginalReleaseDate = 1,
                Name = 2
            });
        }

        Task<OutGenericList<OutUserAlbumAttribute>> ratingsTask = userAlbumAttrsRepository.FindAsync(
            new UserAlbumAttrFindQuery { AlbumId = album.Id },
            orderSpec: new UserAlbumAttrOrderSpecification { CreatedAt = -1, UserName = 2, Rating = 3 },
            pagingOptions: new PagingOptions(100, 0));

        Task<OutUserAlbumAttribute?> currentUserAttrTask = Task.FromResult<OutUserAlbumAttribute?>(null);
        if (query.UserId.HasValue)
        {
            currentUserAttrTask = userAlbumAttrsRepository.FindOneAlbumFromUserAsync(query.UserId.Value, album.Id);
        }

        await Task.WhenAll(songsTask, artistAlbumsTask, ratingsTask, currentUserAttrTask);

        OutAlbum? previousAlbum = null;
        OutAlbum? nextAlbum = null;

        var artistAlbums = await artistAlbumsTask;
        if (artistAlbums.Count > 0)
        {
            var index = artistAlbums.FindIndex(a => a.Id == album.Id);
            if (index > 0) previousAlbum = artistAlbums[index - 1];
            if (index >= 0 && index < artistAlbums.Count - 1) nextAlbum = artistAlbums[index + 1];
        }

        var result = new OutAlbumDataView
        {
            Album = album,
            Songs = await songsTask,
            PreviousAlbum = previousAlbum,
            NextAlbum = nextAlbum,
            CurrentUserAttribute = await currentUserAttrTask,
            Ratings = await ratingsTask
        };

        await cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5), cancellationToken);
        return result;
    }
}