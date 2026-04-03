using Musicx.Contracts.Dto.Responses.Specifics.Ratings;

namespace Musicx.Application.Shared.Interfaces.UseCases.User.Ratings;

public interface IFindUserYearlyRatingsService
{
    Task<IReadOnlyList<OutUserYearlyRating>?> ExecuteAsync(int bucketSize = 5, long? genreId = null, long? userId = null);
    IReadOnlyList<OutUserYearlyRating>? Execute(int bucketSize = 5, long? genreId = null, long? userId = null);
}