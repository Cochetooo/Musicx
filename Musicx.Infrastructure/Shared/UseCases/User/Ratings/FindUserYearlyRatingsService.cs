using System.Text.Json;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Helpers;
using Musicx.Application.Shared.Interfaces.UseCases.User.Ratings;
using Musicx.Contracts.Dto.Responses.Specifics.Ratings;

namespace Musicx.Infrastructure.Shared.UseCases.User.Ratings;

public sealed class FindUserYearlyRatingsService(
    ILoggerFactory loggerFactory,
    HttpClient httpClient)
    : IFindUserYearlyRatingsService
{
    private readonly ILogger _logger = loggerFactory.CreateLogger(nameof(FindUserYearlyRatingsService));

    public async Task<IReadOnlyList<OutUserYearlyRating>?> ExecuteAsync(int bucketSize = 5, long? genreId = null, long? userId = null)
    {
        var endpoint = $"/api/user-album-attrs/yearly-ratings?bucketSize={bucketSize}";

        if (genreId is not null)
        {
            endpoint += $"&genreId={genreId}";
        }

        if (userId is not null)
        {
            endpoint += $"&userId={userId}";
        }

        _logger.LogInformation("🌍🏳️ GET " + endpoint);

        var response = await httpClient.GetStringAsync(endpoint);
        var json = JsonSerializer.Deserialize<IReadOnlyList<OutUserYearlyRating>>(response, JsonHelper.OptionsDefault);

        if (json is null)
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Null response");
            return null;
        }

        _logger.LogInformation($"🌍✅ GET {endpoint} - SUCCESS");

        return json;
    }

    public IReadOnlyList<OutUserYearlyRating>? Execute(int bucketSize = 5, long? genreId = null, long? userId = null)
        => throw new NotImplementedException();
}