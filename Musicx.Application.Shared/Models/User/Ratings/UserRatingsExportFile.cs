namespace Musicx.Application.Shared.Models.User.Ratings;

public sealed record UserRatingsExportFile(
    byte[] Content,
    string FileName,
    string ContentType
);