using System.Text.Json;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Helpers;
using Musicx.Application.Web.Interfaces.UseCases;
using Musicx.Infrastructure.Web.Helpers;

namespace Musicx.Infrastructure.Web.UseCases;

public sealed class UcList<T>(
    HttpClient httpClient,
    ILoggerFactory loggerProvider
    ) : IListUseCase<T> where T : class
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(UcList<T>));
    
    public async Task<List<T>> ExecuteAsync(long skip = 0, long take = 200, 
        bool? filterExact = null, double? filterSimilitude = null, 
        string? filter = null, string? order = null, string query = "")
    {
        var modelName = typeof(T).Name.OutModelToEntity();
        var endpoint = $"/api/{modelName}?skip={skip}&take={take}&query={query}";

        if (null != filter)
        {
            endpoint += $"&filter={filter}";
        }

        if (null != filterExact)
        {
            endpoint += $"&filterExact={filterExact}";
        }

        if (null != filterSimilitude)
        {
            endpoint += $"&filterSimilitude={filterSimilitude}";
        }

        if (null != order)
        {
            endpoint += $"&order={order}";
        }
        
        _logger.LogInformation("🌍🏳️ GET " + endpoint);

        try
        {
            var response = await httpClient.GetStringAsync(endpoint);

            var json = JsonSerializer.Deserialize<List<T>>(response, JsonHelper.OptionsDefault);
            
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

    public List<T> Execute(long skip = 0, long take = 200, 
        bool? filterExact = null, double? filterSimilitude = null, 
        string? filter = null, string? order = null, string query = "")
    {
        throw new NotImplementedException();
    }
}