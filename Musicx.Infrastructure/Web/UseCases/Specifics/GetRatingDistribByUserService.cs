using System.Text.Json;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Helpers;
using Musicx.Application.Web.Interfaces.UseCases.Specifics;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Ratings;
using Musicx.Infrastructure.Web.Helpers;

namespace Musicx.Infrastructure.Web.UseCases.Specifics;

public sealed class GetRatingDistribByUserService<T> (
    ILoggerFactory loggerFactory,
    HttpClient httpClient) 
    : IGetRatingDistribByUserService<T> where T : BaseOutputModel
{
    private readonly ILogger _logger = loggerFactory.CreateLogger(nameof(GetRatingDistribByUserService<T>));
    
    public async Task<OutUserRatingStats?> ExecuteAsync(long userId)
    {
        var modelName = typeof(T).Name.OutModelToEntity();
        var endpoint = $"/api/user-{modelName[..^1]}-attrs/ratings-distrib/{userId}";
        
        _logger.LogInformation("🌍🏳️ GET " + endpoint);
        
        var response = await httpClient.GetStringAsync(endpoint);
        var json = JsonSerializer.Deserialize<OutUserRatingStats>(response, JsonHelper.OptionsDefault);

        if (null == json)
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Null response");
            return null;
        }
        
        _logger.LogInformation($"🌍✅ GET {endpoint} - SUCCESS");

        return json;
    }
    
    public OutUserRatingStats? Execute(long userId)
        => throw new NotImplementedException();
}