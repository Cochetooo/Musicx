using System.Text.Json;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Helpers;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Application.Web.Interfaces.UseCases;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Infrastructure.Web.Helpers;

namespace Musicx.Infrastructure.Web.UseCases;

public sealed class UcFindIn<TIn, TOut>(
    HttpClient httpClient,
    ILoggerFactory loggerProvider
    ) : IFindInUseCase<TIn, TOut> 
    where TIn : BaseInputModel
    where TOut : BaseOutputModel
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(UcFindIn<TIn, TOut>));
    
    public async Task<List<TOut>> ExecuteAsync(
        IEnumerable<long> ids, 
        IJoinSpecification<TIn>? joins = null, 
        OrderSpecification<TIn>? order = null)
    {
        var modelName = typeof(TOut).Name.OutModelToEntity();
        var endpoint = $"/api/{modelName}/by-ids?ids={string.Join("&ids=", ids)}&";
        endpoint += QueryStringHelper.SetUseCaseParameters(joins, order);
        
        _logger.LogInformation("🌍🏳️ GET " + endpoint);

        try
        {
            var response = await httpClient.GetStringAsync(endpoint);

            var json = JsonSerializer.Deserialize<List<TOut>>(response, JsonHelper.OptionsDefault);
            
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

    public List<TOut> Execute(
        IEnumerable<long> ids, 
        IJoinSpecification<TIn>? joins = null, 
        OrderSpecification<TIn>? order = null)
    {
        throw new NotImplementedException();
    }
}