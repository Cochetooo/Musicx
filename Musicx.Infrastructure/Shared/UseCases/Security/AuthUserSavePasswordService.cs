using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Interfaces.UseCases.Security;
using Musicx.Contracts.Dto.Requests.User;

namespace Musicx.Infrastructure.Shared.UseCases.Security;

public sealed class AuthUserSavePasswordService(
    HttpClient httpClient,
    ILoggerFactory loggerFactory) : IAuthUserSavePasswordService
{
    private readonly ILogger _logger = loggerFactory.CreateLogger(nameof(AuthUserSavePasswordService));
    
    public async Task ExecuteAsync(long userId, string password)
    {
        var endpoint = "/api/users/{userId}/change-password";
        
        _logger.LogInformation("🌍🏳️ AUTH USER CHANGE PWD " + endpoint);

        var response = await httpClient.PostAsJsonAsync(endpoint, new InUserPasswordChange
        {
            NewPassword = password,
        });

        _logger.LogInformation(response.IsSuccessStatusCode
            ? $"🌍✅ AUTH USER CHANGE PWD {endpoint} - SUCCESS"
            : $"🌍❌ AUTH USER CHANGE PWD {endpoint} - ERROR : {response.StatusCode} | {response.ReasonPhrase}");
    }
}