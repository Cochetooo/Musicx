using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Interfaces.UseCases;
using Musicx.Infrastructure.Web.Helpers;

namespace Musicx.Infrastructure.Shared.UseCases;

public sealed class DeleteService<T>(
    HttpClient httpClient,
    ILoggerFactory loggerProvider
    ) : IDeleteService<T> where T : class
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(DeleteService<T>));

    public async Task ExecuteAsync(long id)
    {
        var modelName = typeof(T).Name.InModelToEntity();
        var endpoint = $"/api/{modelName}/{id}";
        
        _logger.LogInformation("🌍🏳️ DELETE " + endpoint);
        
        var response = await httpClient.DeleteAsync(endpoint);

        _logger.LogInformation(response.IsSuccessStatusCode
            ? $"🌍✅ DELETE {endpoint} - SUCCESS"
            : $"🌍❌ DELETE {endpoint} - ERROR : {response.StatusCode} | {response.ReasonPhrase}");
    }

    public void Execute(long id)
    {
        throw new NotImplementedException();
    }
}