using System.Text.Json;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Helpers;
using Musicx.Application.Web.Interfaces.UseCases.Artist;
using Musicx.Contracts.Dto.Requests.Artist;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Infrastructure.Web.Helpers;

namespace Musicx.Infrastructure.Web.UseCases.Artist;

public sealed class FindArtistByGenreService(
    HttpClient httpClient,
    ILoggerFactory loggerProvider) : IFindArtistByGenreService
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(FindArtistByGenreService));
    
    public async Task<OutGenericList<OutArtist>> ExecuteAsync(long genreId, PagingOptions? pagingOptions = null)
    {
        var endpoint = $"/api/artists/by-genre/{genreId}?";
        endpoint += QueryStringHelper.SetUseCaseParameters<InArtist>(pagingOptions: pagingOptions);
        
        _logger.LogInformation("🌍🏳️ GET " + endpoint);
        
        var response = await httpClient.GetStringAsync(endpoint);

        if (string.IsNullOrWhiteSpace(response))
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Null response");
            return new OutGenericList<OutArtist>
            {
                Items = [],
                Total = 0,
            };
        }
        
        var json = JsonSerializer.Deserialize<OutGenericList<OutArtist>>(response, JsonHelper.OptionsDefault);

        if (null == json)
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Null response");
            return new OutGenericList<OutArtist>
            {
                Items = [],
                Total = 0
            };
        }
        
        _logger.LogInformation($"🌍✅ GET {endpoint} - SUCCESS");

        return json;
    }
}