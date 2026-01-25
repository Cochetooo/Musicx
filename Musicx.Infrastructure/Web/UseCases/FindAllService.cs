using System.Text.Json;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Helpers;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Application.Web.Interfaces.UseCases;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Infrastructure.Web.Helpers;

namespace Musicx.Infrastructure.Web.UseCases;

public sealed class FindAllService<TIn, TOut>(
    HttpClient httpClient,
    ILoggerFactory loggerProvider
    ) : IFindAllService<TIn, TOut> 
    where TIn : BaseInputModel
    where TOut : BaseOutputModel
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(FindAllService<TIn, TOut>));
    
    public async Task<List<TOut>> ExecuteAsync(
        bool? filterExact = null, 
        double? filterSimilitude = null, 
        string? filter = null, 
        IJoinSpecification<TIn>? joins = null,
        OrderSpecification<TIn>? order = null,
        PagingOptions? pagingOptions = null)
    {
        var modelName = typeof(TOut).Name.OutModelToEntity();
        var endpoint = $"/api/{modelName}?";
        endpoint += QueryStringHelper.SetUseCaseParameters(joins, order, pagingOptions);

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
        bool? filterExact = null, 
        double? filterSimilitude = null, 
        string? filter = null, 
        IJoinSpecification<TIn>? joins = null,
        OrderSpecification<TIn>? order = null,
        PagingOptions? pagingOptions = null)
    {
        throw new NotImplementedException();
    }
}