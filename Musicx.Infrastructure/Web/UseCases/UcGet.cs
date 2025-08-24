using System.Text.Json;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Utilities;
using Musicx.Application.Web.Interfaces.UseCases;
using Musicx.Infrastructure.Web.Helpers;

namespace Musicx.Infrastructure.Web.UseCases;

public sealed class UcGet<T>(
    HttpClient httpClient,
    ILoggerFactory loggerProvider
    ) : IGetUseCase<T> where T : class
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(UcGet<T>));
    
    public async Task<T?> ExecuteAsync(long id, string query = "")
    {
        var modelName = typeof(T).Name.OutModelToEntity();
        var endpoint = $"/api/{modelName}/{id}?query={query}";
        
        _logger.LogInformation("🌍🏳️ GET " + endpoint);
        
        var response = await httpClient.GetStringAsync(endpoint);
        var json = JsonSerializer.Deserialize<T>(response, JsonHelper.OptionsDefault);

        if (null == json)
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Null response");
            return null;
        }
        
        _logger.LogInformation($"🌍✅ GET {endpoint} - SUCCESS");

        return json;
    }

    public T? Execute(long id, string query = "")
    {
        throw new NotImplementedException();
    }
}