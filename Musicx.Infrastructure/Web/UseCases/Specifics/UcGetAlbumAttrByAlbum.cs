using System.Text.Json;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Helpers;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Application.Web.Interfaces.UseCases.Specifics;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Infrastructure.Web.Helpers;

namespace Musicx.Infrastructure.Web.UseCases.Specifics;

public sealed class UcGetAlbumAttrByAlbum(
    HttpClient httpClient,
    ILoggerFactory loggerFactory) : IGetAlbumAttributesByAlbum
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<UcGetAlbumAttrByAlbum>();

    public async Task<OutGenericList<OutUserAlbumAttribute>> ExecuteAsync(
        long albumId, 
        OrderSpecification<InUserAlbumAttribute>? orderSpec = null,
        PagingOptions? pagingOptions = null, 
        CancellationToken cancellationToken = default)
    {
        var endpoint = $"/api/user-album-attrs/by-album/{albumId}?";
        endpoint += QueryStringHelper.SetUseCaseParameters(null, orderSpec, pagingOptions);
        
        _logger.LogInformation("🌍🏳️ GET " + endpoint);
        
        var response = await httpClient.GetStringAsync(endpoint, cancellationToken);

        if (string.IsNullOrWhiteSpace(response))
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Null response");
            return new OutGenericList<OutUserAlbumAttribute>
            {
                Items = [],
                Total = 0
            };
        }
        
        var json = JsonSerializer.Deserialize<OutGenericList<OutUserAlbumAttribute>>(response, JsonHelper.OptionsDefault);

        if (null == json)
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Null response");
            return new OutGenericList<OutUserAlbumAttribute>
            {
                Items = [],
                Total = 0
            };
        }
        
        _logger.LogInformation($"🌍✅ GET {endpoint} - SUCCESS");

        return json;
    }

    public OutGenericList<OutUserAlbumAttribute> Execute(
        long albumId, 
        OrderSpecification<InUserAlbumAttribute>? orderSpec = null,
        PagingOptions? pagingOptions = null, 
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}