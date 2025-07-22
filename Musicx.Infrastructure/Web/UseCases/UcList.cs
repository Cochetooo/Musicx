using System.Text.Json;
using Microsoft.Extensions.Logging;
using Musicx.Application.Web.Interfaces.UseCases;
using Musicx.Infrastructure.Web.Helpers;

namespace Musicx.Infrastructure.Web.UseCases;

public sealed class UcList<T>(
    HttpClient httpClient,
    ILoggerProvider loggerProvider
    ) : IListUseCase<T> where T : class
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(UcList<T>));

    private readonly JsonSerializerOptions options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };
    
    public async Task<List<T>> ExecuteAsync(int skip = 0, int take = 200, string? filter = null, string query = "")
    {
        var modelName = typeof(T).Name.OutModelToEntity();
        var endpoint = $"/api/{modelName}?skip={skip}&take={take}&query={query}";

        if (null != filter)
        {
            endpoint += $"&filter={filter}";
        }
        
        _logger.LogInformation("🌍🏳️ GET " + endpoint);

        try
        {
            var response = await httpClient.GetStringAsync(endpoint);

            var json = JsonSerializer.Deserialize<List<T>>(response, options);
            
            if (null == json)
            {
                _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Null response");
                return [];
            }
        
            _logger.LogInformation($"🌍✅ GET {endpoint} - SUCCESS");

            return json;
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : An error has occured: {ex.Message}");
            return [];
        }
    }

    public List<T> Execute(int skip = 0, int take = 200, string? filter = null, string query = "")
    {
        throw new NotImplementedException();
    }
}