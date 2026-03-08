using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Ratings;

namespace Musicx.Application.Api.Interfaces.Persistence.Repositories.User;

public interface IUserAlbumAttrsRepository : IRepository<InUserAlbumAttribute, OutUserAlbumAttribute>
{
    Task<long> CountByAlbumIdAsync(long albumId);
    
    Task<long> CountByUserIdAsync(long userId, 
        IJoinSpecification<InUserAlbumAttribute>? spec = null,
        long? artistId = null,
        bool? filterExact = null,
        double? filterSimilitude = 0.4,
        string? filter = null);

    Task<long> CountGenreRatingsByUserIdAsync(long userId);
    
    Task DeleteAsync(long userId, long albumId);
    
    Task<IReadOnlyList<OutUserAlbumAttribute>> FindByAlbumIdAsync(long albumId,
        OrderSpecification<InUserAlbumAttribute>? orderSpec = null,
        PagingOptions? pagingOptions = null);
    
    Task<IReadOnlyList<OutUserAlbumAttribute>> FindByUserIdAsync(long userId,
        long? artistId = null,
        bool? filterExact = null,
        double? filterSimilitude = 0.4,
        string? filter = null,
        IJoinSpecification<InUserAlbumAttribute>? spec = null,
        OrderSpecification<InUserAlbumAttribute>? orderSpec = null,
        PagingOptions? pagingOptions = null);

    Task<IReadOnlyList<OutUserGenreRating>> FindGenreRatingsByUserIdAsync(
        long userId,
        bool weighted = false,
        PagingOptions? pagingOptions = null
    );
    
    Task<OutUserAlbumAttribute?> FindOneAlbumFromUserAsync(long userId, long albumId);
    
    Task<IReadOnlyList<OutUserYearlyRating>> GetUserYearlyRatingsAsync(int bucketSize = 5, long? genreId = null, long? userId = null);
    
    Task<OutUserRatingStats> GetUserRatingStatsAsync(long userId);
}