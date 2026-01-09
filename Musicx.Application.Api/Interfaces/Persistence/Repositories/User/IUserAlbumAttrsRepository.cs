using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Ratings;

namespace Musicx.Application.Api.Interfaces.Persistence.Repositories.User;

public interface IUserAlbumAttrsRepository : IRepository<InUserAlbumAttribute, OutUserAlbumAttribute>
{
    Task<long> CountByAlbumIdAsync(long albumId);
    Task<long> CountByUserIdAsync(long userId, 
        IQuerySpecification<InUserAlbumAttribute>? spec = null,
        long? artistId = null,
        bool? filterExact = null,
        double? filterSimilitude = 0.4,
        string? filter = null);
    Task DeleteAsync(long userId, long albumId);
    Task<IReadOnlyList<OutUserAlbumAttribute>> FindByAlbumIdAsync(long albumId,
        long skip = 0,
        long take = 100,
        string? order = null);
    Task<IReadOnlyList<OutUserAlbumAttribute>> FindByUserIdAsync(long userId,
        IQuerySpecification<InUserAlbumAttribute>? spec = null,
        long skip = 0,
        long take = 100,
        long? artistId = null,
        bool? filterExact = null,
        double? filterSimilitude = 0.4,
        string? filter = null,
        string? order = null);
    Task<OutUserAlbumAttribute?> FindOneAlbumFromUserAsync(long userId, long albumId);
    Task<OutUserRatingStats> GetUserRatingStatsAsync(long userId);
}