using System.Text.Json;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Helpers;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Application.Shared.Interfaces.UseCases;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Infrastructure.Web.Helpers;

namespace Musicx.Infrastructure.Shared.UseCases;

public sealed class FindOneByIdService<TIn, TOut>(
    HttpClient httpClient,
    ILoggerFactory loggerProvider
    ) : IFindOneByIdService<TIn, TOut> 
    where TIn : BaseInputModel
    where TOut : BaseOutputModel
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(FindOneByIdService<TIn, TOut>));
    
    public async Task<TOut?> ExecuteAsync(long id, IJoinSpecification<TIn>? joins = null)
    {
        var modelName = typeof(TOut).Name.OutModelToEntity();
        var endpoint = $"/api/{modelName}/{id}?";
        endpoint += QueryStringHelper.SetUseCaseParameters(joins);
        
        _logger.LogInformation("🌍🏳️ GET " + endpoint);
        
        var response = await httpClient.GetStringAsync(endpoint);
        var json = JsonSerializer.Deserialize<TOut>(response, JsonHelper.OptionsDefault);

        if (null == json)
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Null response");
            return null;
        }
        
        _logger.LogInformation($"🌍✅ GET {endpoint} - SUCCESS");

        return json;
    }

    public TOut? Execute(long id, IJoinSpecification<TIn>? joins = null)
    {
        throw new NotImplementedException();
    }
}