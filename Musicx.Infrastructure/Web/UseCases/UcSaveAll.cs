using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Musicx.Application.Web.Interfaces.UseCases;
using Musicx.Contracts.Dto.Requests;
using Musicx.Infrastructure.Web.Helpers;

namespace Musicx.Infrastructure.Web.UseCases;

public sealed class UcSaveAll<T>(
    HttpClient httpClient,
    ILoggerFactory loggerFactory
    ) : ISaveAllUseCase<T> where T : BaseInputModel
{
    private readonly ILogger _logger = loggerFactory.CreateLogger(nameof(UcSaveAll<T>));
    
    public async Task<HttpResponseMessage> ExecuteAsync(IEnumerable<T> entities)
    {
        var modelName = typeof(T).Name.InModelToEntity();
        var endpoint = $"/api/{modelName}/save-all";
        
        _logger.LogInformation("🌍🏳️ SAVE ALL " + endpoint);
        
        var json = JsonSerializer.Serialize(entities);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        _logger.LogInformation("ℹ️ Content sent:\n" + content);

        var response = await httpClient.PostAsync(endpoint, content);

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation($"🌍✅ SAVE ALL {endpoint} - SUCCESS");
        }
        else
        {
            _logger.LogInformation($"🌍❌ SAVE ALL {endpoint} - ERROR : {response.StatusCode} | {response.ReasonPhrase}");
        }

        return response;
    }

    public HttpResponseMessage Execute(IEnumerable<T> entities)
    {
        throw new NotImplementedException();
    }
}