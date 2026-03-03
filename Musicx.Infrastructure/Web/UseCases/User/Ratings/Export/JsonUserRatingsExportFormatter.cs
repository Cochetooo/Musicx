using System.Text;
using System.Text.Json;
using Musicx.Application.Web.Interfaces.Models.User.Ratings;
using Musicx.Application.Web.Interfaces.UseCases.User.Ratings;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Infrastructure.Web.UseCases.User.Ratings.Export;

public sealed class JsonUserRatingsExportFormatter : IUserRatingsExportFormatter
{
    public UserRatingsExportFormat Format => UserRatingsExportFormat.Json;

    public UserRatingsExportFile Export(IEnumerable<OutUserAlbumAttribute> ratings, string userName)
    {
        var payload = ratings.Select(r => new
        {
            Album = r.Album.Name,
            Artist = r.Album.Artist?.Name,
            r.Rating,
            CollectionType = r.CollectionType?.ToString(),
            DiscoveryDate = r.DiscoveryDate?.ToString("yyyy-MM-dd"),
            r.Review
        });

        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        return new UserRatingsExportFile(
            Encoding.UTF8.GetBytes(json),
            $"{Sanitize(userName)}-ratings.json",
            "application/json;charset=utf-8");
    }

    private static string Sanitize(string value)
        => string.Join('-', value.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
}