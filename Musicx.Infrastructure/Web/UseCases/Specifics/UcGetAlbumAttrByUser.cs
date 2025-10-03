using System.Text.Json;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Utilities;
using Musicx.Application.Web.Interfaces.UseCases.Specifics;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Infrastructure.Web.UseCases.Specifics;

public sealed class UcGetAlbumAttrByUser(
    HttpClient httpClient,
    ILoggerFactory loggerFactory) : IGetAlbumAttributesByUser
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<UcGetAlbumAttrByAlbum>();

    public async Task<List<OutUserAlbumAttribute>> ExecuteAsync(long userId)
    {
        var endpoint = $"/api/user-album-attrs/by-user/{userId}";
        
        _logger.LogInformation("🌍🏳️ GET " + endpoint);
        
        var response = await httpClient.GetStringAsync(endpoint);

        if (string.IsNullOrWhiteSpace(response))
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Null response");
            return [];
        }
        
        var json = JsonSerializer.Deserialize<List<OutUserAlbumAttribute>>(response, JsonHelper.OptionsDefault);

        if (null == json)
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Null response");
            return [];
        }
        
        _logger.LogInformation($"🌍✅ GET {endpoint} - SUCCESS");

        return json;
    }

    public List<OutUserAlbumAttribute> Execute(long albumId)
    {
        throw new NotImplementedException();
    }
}