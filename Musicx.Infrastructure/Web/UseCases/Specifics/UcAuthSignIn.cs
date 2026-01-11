using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Musicx.Application.Web.Interfaces.UseCases.Specifics;
using Musicx.Contracts.Dto.Requests.Specifics;

namespace Musicx.Infrastructure.Web.UseCases.Specifics;

public sealed class UcAuthSignIn(
    HttpClient httpClient,
    ILoggerFactory loggerFactory) : IAuthSignInUseCase
{
    private readonly ILogger _logger = loggerFactory.CreateLogger(nameof(UcAuthSignIn));
    
    public async Task<string?> ExecuteAsync(SignInRequest request)
    {
        var endpoint = "/api/auth/signin";
        
        _logger.LogInformation("🌍🏳️ AUTH SIGNIN " + endpoint);
        
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        _logger.LogInformation("ℹ️ Content sent:\n" + content);
        
        var response = await httpClient.PostAsync(endpoint, content);
        var token = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation($"🌍✅ AUTH SIGNIN {endpoint} - SUCCESS");
            _logger.LogInformation($"🔑 Token: {token}");
        }
        else
        {
            _logger.LogInformation($"🌍❌ AUTH SIGNIN {endpoint} - ERROR : {response.StatusCode} | {token}");
        }

        return token;
    }

    public string? Execute(SignInRequest request) 
        => throw new NotImplementedException();
}