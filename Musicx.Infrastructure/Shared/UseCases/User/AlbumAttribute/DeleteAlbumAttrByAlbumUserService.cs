using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Interfaces.UseCases.User.AlbumAttribute;

namespace Musicx.Infrastructure.Shared.UseCases.User.AlbumAttribute;

public sealed class DeleteAlbumAttrByAlbumUserService(
    HttpClient httpClient,
    ILoggerFactory loggerProvider) : IDeleteAlbumAttrByAlbumUserService
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(DeleteAlbumAttrByAlbumUserService));
    
    public async Task ExecuteAsync(long userId, long albumId)
    {
        var endpoint = $"/api/user-album-attrs/{userId}/{albumId}";
        
        _logger.LogInformation("🌍🏳️ DELETE " + endpoint);
        
        var response = await httpClient.DeleteAsync(endpoint);

        _logger.LogInformation(response.IsSuccessStatusCode
            ? $"🌍✅ DELETE {endpoint} - SUCCESS"
            : $"🌍❌ DELETE {endpoint} - ERROR : {response.StatusCode} | {response.ReasonPhrase}");
    }
}