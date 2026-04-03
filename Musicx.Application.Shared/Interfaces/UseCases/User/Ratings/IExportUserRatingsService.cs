using Musicx.Application.Shared.Models.User.Ratings;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.User;

namespace Musicx.Application.Shared.Interfaces.UseCases.User.Ratings;

public interface IExportUserRatingsService
{
    UserRatingsExportFile Execute(
        IEnumerable<OutUserAlbumAttribute> ratings,
        string userName,
        UserRatingsExportFormat format
    );
}