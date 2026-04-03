using System.Text.Json;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Helpers;
using Musicx.Application.Shared.Interfaces.UseCases.User.FavoriteAlbum;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Contracts.Dto.Responses.User;

namespace Musicx.Infrastructure.Shared.UseCases.User.FavoriteAlbum;

public sealed class FindFavouriteAlbumByUserService(
    HttpClient httpClient,
    ILoggerFactory loggerFactory) : IFindFavouriteAlbumByUserService
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<FindFavouriteAlbumByUserService>();

    public async Task<OutGenericList<OutUserFavoriteAlbum>> ExecuteAsync(long userId,
        CancellationToken cancellationToken = default)
    {
        var endpoint = $"/api/user-fav-album/by-user/{userId}";
        
        _logger.LogInformation("🌍🏳️ GET " + endpoint);
        
        var response = await httpClient.GetStringAsync(endpoint, cancellationToken);

        if (string.IsNullOrWhiteSpace(response))
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Null response");
            return new OutGenericList<OutUserFavoriteAlbum>
            {
                Items = [],
                Total = 0
            };
        }
        
        var json = JsonSerializer.Deserialize<OutGenericList<OutUserFavoriteAlbum>>(response, JsonHelper.OptionsDefault);

        if (null == json)
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Null response");
            return new OutGenericList<OutUserFavoriteAlbum>
            {
                Items = [],
                Total = 0
            };
        }
        
        _logger.LogInformation($"🌍✅ GET {endpoint} - SUCCESS");

        return json;
    }

    public OutGenericList<OutUserFavoriteAlbum> Execute(
        long userId, 
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}