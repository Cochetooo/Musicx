using Musicx.Application.Web.Interfaces.Models.User.Ratings;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Application.Web.Interfaces.UseCases.User.Ratings;

public interface IExportUserRatingsService
{
    UserRatingsExportFile Execute(
        IEnumerable<OutUserAlbumAttribute> ratings,
        string userName,
        UserRatingsExportFormat format
    );
}