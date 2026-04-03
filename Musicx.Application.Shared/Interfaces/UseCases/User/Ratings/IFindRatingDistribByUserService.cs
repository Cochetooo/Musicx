using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Ratings;

namespace Musicx.Application.Shared.Interfaces.UseCases.User.Ratings;

public interface IFindRatingDistribByUserService<T> where T : BaseOutputModel
{
    Task<OutUserRatingStats?> ExecuteAsync(long userId);
    OutUserRatingStats? Execute(long userId);
}