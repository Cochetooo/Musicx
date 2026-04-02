using Musicx.Application.Api.Interfaces.Caching;
using Musicx.Application.Api.Interfaces.DataViews;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.User;
using Musicx.Application.API.Persistence.Queries;
using Musicx.Application.Shared.Enums;
using Musicx.Contracts.Dto.Responses.Specifics.Users;
using Musicx.Infrastructure.API.Persistence.Specifications.User;

namespace Musicx.Infrastructure.API.DataViews;

/// <summary>
/// Builds a consolidated User DataView from multiple repositories.
/// </summary>
/// <since>0.7.4</since>
public sealed class UserDataViewBuilder(
    IUserRepository userRepository,
    IUserAlbumAttrsRepository userAlbumAttrsRepository,
    IDataViewCacheProvider cache,
    IDataViewCacheKeyFactory cacheKeyFactory) : IUserDataViewBuilder
{
    public async Task<OutUserDataView?> BuildAsync(UserDataViewQuery query, CancellationToken cancellationToken = default)
    {
        var cacheKey = cacheKeyFactory.BuildUserDataViewKey(query.UserId, query.CurrentUserId);
        var cached = await cache.GetAsync<OutUserDataView>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            return cached;
        }

        var userTask = userRepository.FindOneByIdAsync(query.UserId, new UserJoinSpecification
        {
            IncludeRoles = true
        });

        var ratingStatsTask = userAlbumAttrsRepository.GetUserRatingStatsAsync(query.UserId);
        var topGenresTask = userAlbumAttrsRepository.FindGenreRatingsByUserIdAsync(query.UserId, true, new PagingOptions(5, 0));
        var bestRatingsTask = userAlbumAttrsRepository.FindAsync(
            new UserAlbumAttrFindQuery { UserId = query.UserId },
            joinSpec: new UserAlbumAttrJoinSpecification { IncludeAlbumArtists = true },
            orderSpec: new UserAlbumAttrOrderSpecification { Rating = -1, AlbumName = -2 },
            pagingOptions: new PagingOptions(6, 0));

        await Task.WhenAll(userTask, ratingStatsTask, topGenresTask, bestRatingsTask);

        var user = await userTask;
        if (user is null)
        {
            return null;
        }

        var result = new OutUserDataView
        {
            User = user,
            RatingStats = await ratingStatsTask,
            TopGenres = await topGenresTask,
            FavoriteAlbums = (await bestRatingsTask).Items.Select(x => x.Album).ToList()
        };

        await cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5), cancellationToken);
        return result;
    }
}