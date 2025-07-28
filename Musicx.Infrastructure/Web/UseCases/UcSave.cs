using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Musicx.Application.Web.Interfaces.UseCases;

using Musicx.Infrastructure.Web.Helpers;

namespace Musicx.Infrastructure.Web.UseCases;

public sealed class UcSave<T>(
    HttpClient httpClient,
    ILoggerProvider loggerProvider
    ) : ISaveUseCase<T> where T : class
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(UcSave<T>));
    
    public async Task ExecuteAsync(T entity)
    {
        var modelName = typeof(T).Name.InModelToEntity();
        var endpoint = $"/api/{modelName}";
        
        _logger.LogInformation("🌍🏳️ SAVE " + endpoint);
        
        var json = JsonSerializer.Serialize(entity);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        _logger.LogInformation("ℹ️ Content sent:\n" + content);

        var response = await httpClient.PostAsync(endpoint, content);

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation($"🌍✅ SAVE {endpoint} - SUCCESS");
        }
        else
        {
            _logger.LogInformation($"🌍❌ SAVE {endpoint} - ERROR : {response.StatusCode} | {response.ReasonPhrase}");
        }
    }

    public void Execute(T entity)
    {
        throw new NotImplementedException();
    }
}