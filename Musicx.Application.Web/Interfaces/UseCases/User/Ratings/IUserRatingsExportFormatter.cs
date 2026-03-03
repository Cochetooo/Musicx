using Musicx.Application.Web.Interfaces.Models.User.Ratings;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Application.Web.Interfaces.UseCases.User.Ratings;

public interface IUserRatingsExportFormatter
{
    UserRatingsExportFormat Format { get; }
    UserRatingsExportFile Export(IEnumerable<OutUserAlbumAttribute> ratings, string userName);
}