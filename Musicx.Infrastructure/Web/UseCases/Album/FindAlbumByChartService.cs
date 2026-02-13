using System.Text.Json;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Helpers;
using Musicx.Application.Web.Interfaces.UseCases.Album;
using Musicx.Contracts.Dto.Requests.Specifics;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Infrastructure.Web.Helpers;

namespace Musicx.Infrastructure.Web.UseCases.Album;

public sealed class FindAlbumByChartService(
    HttpClient httpClient,
    ILoggerFactory loggerFactory) : IFindAlbumByChartService
{
    private readonly ILogger _logger = loggerFactory.CreateLogger(nameof(FindAlbumByChartService));
    
    public async Task<OutAlbumList> ExecuteAsync(AlbumChartQuery query)
    {
        var endpoint = $"/api/albums/by-chart" + query.ToQueryString();
        
        _logger.LogInformation("🌍🏳️ GET " + endpoint);
        
        var response = await httpClient.GetStringAsync(endpoint);

        if (string.IsNullOrWhiteSpace(response))
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Null response");
            return new();
        }
        
        var json = JsonSerializer.Deserialize<OutAlbumList>(response, JsonHelper.OptionsDefault);

        if (null == json)
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Null response");
            return new();
        }
        
        _logger.LogInformation($"🌍✅ GET {endpoint} - SUCCESS");

        return json;
    }

    public OutAlbumList Execute(AlbumChartQuery query)
    {
        throw new NotImplementedException();
    }
}