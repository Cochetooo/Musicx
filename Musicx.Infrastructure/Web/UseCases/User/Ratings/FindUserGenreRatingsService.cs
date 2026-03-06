using System.Text.Json;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Helpers;
using Musicx.Application.Web.Interfaces.UseCases.User.Ratings;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Contracts.Dto.Responses.Specifics.Ratings;

namespace Musicx.Infrastructure.Web.UseCases.User.Ratings;

public sealed class FindUserGenreRatingsService(
    HttpClient httpClient,
    ILoggerFactory loggerFactory) : IFindUserGenreRatingsService
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<FindUserGenreRatingsService>();

    public async Task<OutGenericList<OutUserGenreRating>> ExecuteAsync(
        long userId,
        bool weighted = false,
        PagingOptions? pagingOptions = null,
        CancellationToken cancellationToken = default)
    {
        var endpoint = $"/api/user-album-attrs/genre-ratings/{userId}?weighted={weighted.ToString().ToLower()}";

        if (pagingOptions is not null)
        {
            endpoint += $"&paging.take={pagingOptions.Take}&paging.skip={pagingOptions.Skip}";
        }

        _logger.LogInformation("🌍🏳️ GET " + endpoint);

        var response = await httpClient.GetStringAsync(endpoint, cancellationToken);

        if (string.IsNullOrWhiteSpace(response))
        {
            return new OutGenericList<OutUserGenreRating>
            {
                Items = [],
                Total = 0
            };
        }

        var json = JsonSerializer.Deserialize<OutGenericList<OutUserGenreRating>>(response, JsonHelper.OptionsDefault);

        if (json is null)
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Null response");
            return new OutGenericList<OutUserGenreRating>
            {
                Items = [],
                Total = 0
            };
        }

        _logger.LogInformation($"🌍✅ GET {endpoint} - SUCCESS");
        return json;
    }

    public OutGenericList<OutUserGenreRating> Execute(
        long userId,
        bool weighted = false,
        PagingOptions? pagingOptions = null,
        CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
}