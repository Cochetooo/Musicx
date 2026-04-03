using Musicx.Application.Shared.Models.User.Ratings;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Application.Shared.Interfaces.UseCases.User.Ratings;

public interface IUserRatingsExportFormatter
{
    UserRatingsExportFormat Format { get; }
    UserRatingsExportFile Export(IEnumerable<OutUserAlbumAttribute> ratings, string userName);
}