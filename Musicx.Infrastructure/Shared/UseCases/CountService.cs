using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Interfaces.UseCases;
using Musicx.Contracts.Dto.Responses;
using Musicx.Infrastructure.Web.Helpers;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Musicx.Infrastructure.Shared.UseCases;

public sealed class CountService<T>(
    HttpClient httpClient,
    ILoggerFactory loggerFactory
    ) : ICountService<T> where T : BaseOutputModel
{
    private readonly ILogger _logger = loggerFactory.CreateLogger(nameof(CountService<T>));
    
    public async Task<long> ExecuteAsync()
    {
        var modelName = typeof(T).Name.OutModelToEntity();
        var endpoint = $"/api/{modelName}/count";
        
        _logger.LogInformation("🌍🏳️ COUNT " + endpoint);
        
        var response = await httpClient.GetAsync(endpoint);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            if (long.TryParse(content, out var count))
            {
                _logger.LogInformation($"🌍✅ COUNT {endpoint} - SUCCESS");
                return count;
            }
        }

        _logger.LogInformation($"🌍❌ COUNT {endpoint} - ERROR : {response.StatusCode} | {response.ReasonPhrase}");
        return 0;
    }

    public long Execute()
        => throw new NotImplementedException();
}