using System.Text.Json;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Helpers;
using Musicx.Application.Shared.Interfaces.UseCases.User.AlbumAttribute;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Infrastructure.Shared.UseCases.User.AlbumAttribute;

public sealed class FindAlbumAttrByAlbumUserService(
    HttpClient httpClient,
    ILoggerFactory loggerFactory) : IFindAlbumAttributesByAlbumUserService
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<FindAlbumAttrByAlbumUserService>();
    
    public async Task<OutUserAlbumAttribute?> ExecuteAsync(long userId, long albumId)
    {
        var endpoint = $"/api/user-album-attrs/by-user/{userId}/{albumId}";
        
        _logger.LogInformation("🌍🏳️ GET " + endpoint);
        
        var response = await httpClient.GetStringAsync(endpoint);

        if (string.IsNullOrWhiteSpace(response))
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Null response");
            return null;
        }
        
        var json = JsonSerializer.Deserialize<OutUserAlbumAttribute>(response, JsonHelper.OptionsDefault);

        if (null == json)
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Null response");
            return null;
        }
        
        _logger.LogInformation($"🌍✅ GET {endpoint} - SUCCESS");

        return json;
    }

    public OutUserAlbumAttribute? Execute(long userId, long albumId)
    {
        throw new NotImplementedException();
    }
}