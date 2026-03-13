using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Helpers;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Application.Web.Interfaces.UseCases.User.AlbumAttribute;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Infrastructure.Web.Helpers;

namespace Musicx.Infrastructure.Web.UseCases.User.AlbumAttribute;

public sealed class FindAlbumAttrByUserService(
    HttpClient httpClient,
    ILoggerFactory loggerFactory) : IFindAlbumAttributesByUserService
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<FindAlbumAttrByAlbumService>();

    public async Task<OutGenericList<OutUserAlbumAttribute>> ExecuteAsync(long userId, 
        long? artistId = null,
        bool? filterExact = null,
        double? filterSimilitude = 0.4,
        string? filter = null,
        IJoinSpecification<InUserAlbumAttribute>? joins = null,
        OrderSpecification<InUserAlbumAttribute>? order = null,
        PagingOptions? pagingOptions = null,
        CancellationToken cancellationToken = default)
    {
        var endpoint = $"/api/user-album-attrs?userId={userId}&";
        endpoint += QueryStringHelper.SetUseCaseParameters(joins, order, pagingOptions);

        if (artistId is not null)
        {
            endpoint += $"&artistId={artistId}";
        }

        if (filter is not null)
        {
            endpoint += $"&filter={filter}";
            endpoint += $"&filterExact={filterExact ?? false}";
            endpoint += $"&filterSimilitude={filterSimilitude?.ToString(CultureInfo.InvariantCulture) ?? "0.4"}";
        }
        
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
        long userId, 
        long? artistId = null,
        bool? filterExact = null,
        double? filterSimilitude = 0.4,
        string? filter = null,
        IJoinSpecification<InUserAlbumAttribute>? joins = null,
        OrderSpecification<InUserAlbumAttribute>? order = null,
        PagingOptions? pagingOptions = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}