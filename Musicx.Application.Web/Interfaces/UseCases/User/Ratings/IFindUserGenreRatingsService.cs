using Musicx.Application.Shared.Enums;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Contracts.Dto.Responses.Specifics.Ratings;

namespace Musicx.Application.Web.Interfaces.UseCases.User.Ratings;

public interface IFindUserGenreRatingsService
{
    Task<OutGenericList<OutUserGenreRating>> ExecuteAsync(
        long userId,
        bool weighted = false,
        PagingOptions? pagingOptions = null,
        CancellationToken cancellationToken = default);

    OutGenericList<OutUserGenreRating> Execute(
        long userId,
        bool weighted = false,
        PagingOptions? pagingOptions = null,
        CancellationToken cancellationToken = default);
}