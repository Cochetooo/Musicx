using Musicx.Application.Web.Interfaces.Models.User.Ratings;
using Musicx.Application.Web.Interfaces.UseCases.User.Ratings;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Infrastructure.Web.UseCases.User.Ratings;

public sealed class ExportUserRatingsService(IEnumerable<IUserRatingsExportFormatter> formatters) : IExportUserRatingsService
{
    private readonly Dictionary<UserRatingsExportFormat, IUserRatingsExportFormatter> _formatters =
        formatters.ToDictionary(x => x.Format);

    public UserRatingsExportFile Execute(
        IEnumerable<OutUserAlbumAttribute> ratings,
        string userName,
        UserRatingsExportFormat format)
    {
        if (!_formatters.TryGetValue(format, out var formatter))
        {
            throw new NotSupportedException($"Export format '{format}' is not supported.");
        }

        return formatter.Export(ratings, userName);
    }
}