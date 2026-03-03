namespace Musicx.Application.Web.Interfaces.Models.User.Ratings;

public sealed record UserRatingsExportFile(
    byte[] Content,
    string FileName,
    string ContentType
);