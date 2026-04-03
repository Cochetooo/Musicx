using System.Text.Json;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Helpers;
using Musicx.Application.Shared.Interfaces.UseCases.User;
using Musicx.Contracts.Dto.Responses.Specifics.Users;

namespace Musicx.Infrastructure.Shared.UseCases.User;

public sealed class FindUserDataViewService(
    HttpClient httpClient,
    ILoggerFactory loggerProvider) : IFindUserDataViewService
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(FindUserDataViewService));

    public async Task<OutUserDataView?> ExecuteAsync(long userId, long? currentUserId = null)
    {
        var endpoint = $"/api/users/{userId}/data-view";
        if (currentUserId.HasValue)
        {
            endpoint += $"?userId={currentUserId.Value}";
        }

        _logger.LogInformation("🌍🏳️ GET " + endpoint);

        var response = await httpClient.GetStringAsync(endpoint);

        if (string.IsNullOrWhiteSpace(response))
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Null response");
            return null;
        }

        var json = JsonSerializer.Deserialize<OutUserDataView>(response, JsonHelper.OptionsDefault);
        if (json is null)
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Deserialization failed");
            return null;
        }

        _logger.LogInformation($"🌍✅ GET {endpoint} - SUCCESS");
        return json;
    }
}