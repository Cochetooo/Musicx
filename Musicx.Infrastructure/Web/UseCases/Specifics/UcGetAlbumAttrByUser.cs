using System.Text.Json;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Utilities;
using Musicx.Application.Web.Interfaces.UseCases.Specifics;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;

namespace Musicx.Infrastructure.Web.UseCases.Specifics;

public sealed class UcGetAlbumAttrByUser(
    HttpClient httpClient,
    ILoggerFactory loggerFactory) : IGetAlbumAttributesByUser
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<UcGetAlbumAttrByAlbum>();

    public async Task<OutGenericList<OutUserAlbumAttribute>> ExecuteAsync(long userId, 
        long skip = 0, long take = 100, CancellationToken token = default)
    {
        var endpoint = $"/api/user-album-attrs/by-user/{userId}?skip={skip}&take={take}";
        
        _logger.LogInformation("🌍🏳️ GET " + endpoint);
        
        var response = await httpClient.GetStringAsync(endpoint, token);

        if (string.IsNullOrWhiteSpace(response))
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Null response");
            return new OutGenericList<OutUserAlbumAttribute>
            {
                Items = [],
                Total = 0
            };
        }
        
        var json = JsonSerializer.Deserialize<OutGenericList<OutUserAlbumAttribute>>(response, JsonHelper.OptionsDefault);

        if (null == json)
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Null response");
            return new OutGenericList<OutUserAlbumAttribute>
            {
                Items = [],
                Total = 0
            };
        }
        
        _logger.LogInformation($"🌍✅ GET {endpoint} - SUCCESS");

        return json;
    }

    public OutGenericList<OutUserAlbumAttribute> Execute(long albumId, long skip = 0, long take = 100)
    {
        throw new NotImplementedException();
    }
}