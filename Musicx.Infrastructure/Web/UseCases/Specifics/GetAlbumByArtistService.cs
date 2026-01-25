using System.Text.Json;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Helpers;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Application.Web.Interfaces.UseCases.Specifics;
using Musicx.Contracts.Dto.Requests.Album;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Infrastructure.Web.Helpers;

namespace Musicx.Infrastructure.Web.UseCases.Specifics;

public sealed class GetAlbumByArtistService(
    HttpClient httpClient,
    ILoggerFactory loggerProvider) : IGetAlbumByArtistService
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(GetAlbumByArtistService));
    
    public async Task<OutAlbumList> ExecuteAsync(
        long artistId, 
        IJoinSpecification<InAlbum>? joinSpec = null,
        OrderSpecification<InAlbum>? orderSpec = null)
    {
        var endpoint = $"/api/albums/by-artist/{artistId}?";
        endpoint += QueryStringHelper.SetUseCaseParameters(joinSpec, orderSpec);
        
        _logger.LogInformation("🌍🏳️ GET " + endpoint);
        
        var response = await httpClient.GetStringAsync(endpoint);

        if (string.IsNullOrWhiteSpace(response))
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Null response");
            return new OutAlbumList
            {
                Items = [],
                Total = 0,
                AverageRating = null,
            };
        }
        
        var json = JsonSerializer.Deserialize<OutAlbumList>(response, JsonHelper.OptionsDefault);

        if (null == json)
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Null response");
            return new OutAlbumList
            {
                Items = [],
                Total = 0,
                AverageRating = null,
            };
        }
        
        _logger.LogInformation($"🌍✅ GET {endpoint} - SUCCESS");

        return json;
    }

    public OutAlbumList Execute(
        long artistId, 
        IJoinSpecification<InAlbum>? joinSpec = null,
        OrderSpecification<InAlbum>? orderSpecification = null)
    {
        throw new NotImplementedException();
    }
}