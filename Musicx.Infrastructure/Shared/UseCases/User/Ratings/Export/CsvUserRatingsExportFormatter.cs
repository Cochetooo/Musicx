using System.Text;
using Musicx.Application.Shared.Interfaces.UseCases.User.Ratings;
using Musicx.Application.Shared.Models.User.Ratings;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.User;

namespace Musicx.Infrastructure.Shared.UseCases.User.Ratings.Export;

public sealed class CsvUserRatingsExportFormatter : IUserRatingsExportFormatter
{
    public UserRatingsExportFormat Format => UserRatingsExportFormat.Csv;
    public UserRatingsExportFile Export(IEnumerable<OutUserAlbumAttribute> ratings, string userName)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Album,Artists,Rating,CollectionType,DiscoveryDate,Review");

        foreach (var rating in ratings)
        {
            sb.AppendLine(string.Join(",",
                Escape(rating.Album.Name),
                Escape(rating.Album.Artist?.Name ?? string.Empty),
                Escape(rating.Rating?.ToString() ?? string.Empty),
                Escape(rating.CollectionType?.ToString() ?? string.Empty),
                Escape(rating.DiscoveryDate?.ToString("yyyy-MM-dd") ?? string.Empty),
                Escape(rating.Review ?? string.Empty)
            ));
        }
        
        return new UserRatingsExportFile(
            Encoding.UTF8.GetBytes(sb.ToString()),
            $"{Sanitize(userName)}-ratings.csv",
            "text/csv;charset=utf-8");
    }
    
    private static string Escape(string value)
    {
        if (value.Contains('"') || value.Contains(',') || value.Contains('\n') || value.Contains('\r'))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        return value;
    }

    private static string Sanitize(string value)
        => string.Join('-', value.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
}