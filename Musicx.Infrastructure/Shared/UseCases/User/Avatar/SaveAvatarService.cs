using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Interfaces.UseCases.User.Avatar;

namespace Musicx.Infrastructure.Shared.UseCases.User.Avatar;

public sealed class SaveAvatarService(
    HttpClient httpClient,
    ILoggerFactory loggerProvider) : ISaveAvatarService
{
    private readonly ILogger _logger =
        loggerProvider.CreateLogger(nameof(SaveAvatarService));
    
    public async Task<string> ExecuteAsync(Stream fileStream, string contentType)
    {
        var endpoint = "/api/users/save-avatar";

        _logger.LogInformation("🌍🏳️ SAVE AVATAR " + endpoint);

        using var content = new MultipartFormDataContent();
        using var streamContent = new StreamContent(fileStream);

        streamContent.Headers.ContentType =
            new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

        content.Add(streamContent, "avatar", "avatar");

        _logger.LogInformation("ℹ️ Multipart avatar content ready");

        var response = await httpClient.PostAsync(endpoint, content);

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation($"🌍✅ SAVE AVATAR {endpoint} - SUCCESS");
            return await response.Content.ReadAsStringAsync();
        }

        _logger.LogError(
            $"🌍❌ SAVE AVATAR {endpoint} - ERROR : {response.StatusCode} | {response.ReasonPhrase}");

        throw new HttpRequestException("Avatar upload failed");
    }

    public string Execute(Stream fileStream, string contentType)
    {
        throw new NotImplementedException();
    }
}