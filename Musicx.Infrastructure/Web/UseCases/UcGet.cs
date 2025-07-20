using System.Text.Json;
using Microsoft.Extensions.Logging;
using Musicx.Application.Web.Interfaces.UseCases;
using Musicx.Infrastructure.Web.Helpers;

namespace Musicx.Infrastructure.Web.UseCases;

public sealed class UcGet<T>(
    HttpClient httpClient,
    ILoggerProvider loggerProvider
    ) : IGetUseCase<T> where T : class
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(UcGet<T>));
    
    private readonly JsonSerializerOptions options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };
    
    public async Task<T?> ExecuteAsync(long id)
    {
        var modelName = typeof(T).Name.OutModelToEntity();
        var endpoint = $"/api/{modelName}/{id}";
        
        _logger.LogInformation("🌍🏳️ GET " + endpoint);
        
        var response = await httpClient.GetStringAsync(endpoint);
        var json = JsonSerializer.Deserialize<T>(response, options);

        if (null == json)
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Null response");
            return null;
        }
        
        _logger.LogInformation($"🌍✅ GET {endpoint} - SUCCESS");

        return json;
    }

    public T? Execute(long id)
    {
        throw new NotImplementedException();
    }
}