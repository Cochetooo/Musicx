using System.Text.Json;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Helpers;
using Musicx.Application.Web.Interfaces.UseCases;
using Musicx.Infrastructure.Web.Helpers;

namespace Musicx.Infrastructure.Web.UseCases;

public sealed class UcFindIn<T>(
    HttpClient httpClient,
    ILoggerFactory loggerProvider
    ) : IFindInUseCase<T> where T : class
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(UcFindIn<T>));
    
    public async Task<List<T>> ExecuteAsync(IEnumerable<long> ids, string query = "")
    {
        var modelName = typeof(T).Name.OutModelToEntity();
        var endpoint = $"/api/{modelName}/by-ids?query={query}&ids={string.Join("&ids=", ids)}";
        
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

    public List<T> Execute(IEnumerable<long> ids, string query = "")
    {
        throw new NotImplementedException();
    }
}