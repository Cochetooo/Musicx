using System.Text.Json;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Helpers;
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
        string query = "",
        long skip = 0, long take = 100, CancellationToken token = default,
        long? artistId = null, string? filter = null)
    {
        var endpoint = $"/api/user-album-attrs/by-user/{userId}?skip={skip}&take={take}&query={query}";

        if (artistId is not null)
        {
            endpoint += $"&artistId={artistId}";
        }

        if (filter is not null)
        {
            endpoint += $"&filter={filter}";
        }
        
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

    public OutGenericList<OutUserAlbumAttribute> Execute(long albumId, long skip = 0, long take = 100,
        long? artistId = null)
    {
        throw new NotImplementedException();
    }
}