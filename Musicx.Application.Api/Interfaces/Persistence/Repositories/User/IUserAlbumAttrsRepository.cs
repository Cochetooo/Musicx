using Musicx.Application.API.Persistence.Queries;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Ratings;

namespace Musicx.Application.Api.Interfaces.Persistence.Repositories.User;

public interface IUserAlbumAttrsRepository : IRepository<InUserAlbumAttribute, OutUserAlbumAttribute>
{
    Task<long> CountGenreRatingsByUserIdAsync(long userId);
    
    Task DeleteAsync(long userId, long albumId);

    Task<IReadOnlyList<OutUserGenreRating>> FindGenreRatingsByUserIdAsync(
        long userId,
        bool weighted = false,
        PagingOptions? pagingOptions = null
    );
    
    Task<OutUserAlbumAttribute?> FindOneAlbumFromUserAsync(long userId, long albumId);
    
    Task<IReadOnlyList<OutUserYearlyRating>> GetUserYearlyRatingsAsync(int bucketSize = 5, long? genreId = null, long? userId = null);
    
    Task<OutUserRatingStats> GetUserRatingStatsAsync(long userId);
}