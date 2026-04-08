using System.Globalization;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Helpers;
using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Album;
using Musicx.Contracts.Dto.Requests.Artist;
using Musicx.Contracts.Dto.Requests.Song;
using Musicx.Contracts.Dto.Requests.Specifics;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Artist;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Contracts.Dto.Responses.User;
using Musicx.Infrastructure.Web.Helpers;

namespace Musicx.Infrastructure.Shared.Clients;

public sealed class ApiClient(HttpClient httpClient, ILoggerFactory loggerFactory) : IApiClient
{
    private readonly ILogger _logger = loggerFactory.CreateLogger(nameof(ApiClient));

    public async Task<OutGenericList<TOut>> FindAsync<TIn, TOut>(IFindQuery<TIn>? query = null,
        IJoinSpecification<TIn>? joins = null,
        OrderSpecification<TIn>? order = null,
        PagingOptions? pagingOptions = null)
        where TIn : BaseInputModel
        where TOut : BaseOutputModel
    {
        var modelName = typeof(TOut).Name.OutModelToEntity();
        var endpoint = $"/api/{modelName}?{QueryStringHelper.SetUseCaseParameters(joins, order, pagingOptions)}";
        endpoint = AppendFindQuery(endpoint, query);

        _logger.LogInformation("🌍🏳️ GET {Endpoint}", endpoint);

        try
        {
            var response = await httpClient.GetStringAsync(endpoint);
            var json = JsonSerializer.Deserialize<OutGenericList<TOut>>(response, JsonHelper.OptionsDefault);
            return json ?? new OutGenericList<TOut> { Items = [], Total = 0 };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "🌍⚠️ GET {Endpoint} failed", endpoint);
            return new OutGenericList<TOut> { Items = [], Total = 0 };
        }
    }

    public async Task<List<TOut>> FindInAsync<TIn, TOut>(IEnumerable<long> ids,
        IJoinSpecification<TIn>? joins = null,
        OrderSpecification<TIn>? order = null)
        where TIn : BaseInputModel
        where TOut : BaseOutputModel
    {
        var modelName = typeof(TOut).Name.OutModelToEntity();
        var endpoint = $"/api/{modelName}/by-ids?ids={string.Join("&ids=", ids)}&{QueryStringHelper.SetUseCaseParameters(joins, order)}";

        try
        {
            var response = await httpClient.GetStringAsync(endpoint);
            return JsonSerializer.Deserialize<List<TOut>>(response, JsonHelper.OptionsDefault) ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "🌍⚠️ GET {Endpoint} failed", endpoint);
            return [];
        }
    }

    public async Task<TOut?> FindByIdAsync<TIn, TOut>(long id, IJoinSpecification<TIn>? joins = null)
        where TIn : BaseInputModel
        where TOut : BaseOutputModel
    {
        var modelName = typeof(TOut).Name.OutModelToEntity();
        var endpoint = $"/api/{modelName}/{id}?{QueryStringHelper.SetUseCaseParameters(joins)}";
        var response = await httpClient.GetStringAsync(endpoint);
        return JsonSerializer.Deserialize<TOut>(response, JsonHelper.OptionsDefault);
    }

    public async Task<HttpResponseMessage> SaveAsync<T>(T entity) where T : class
    {
        var modelName = typeof(T).Name.InModelToEntity();
        var endpoint = $"/api/{modelName}";
        var content = new StringContent(JsonSerializer.Serialize(entity), Encoding.UTF8, "application/json");
        return await httpClient.PostAsync(endpoint, content);
    }

    public async Task<HttpResponseMessage> SaveAllAsync<T>(IEnumerable<T> entities) where T : BaseInputModel
    {
        var modelName = typeof(T).Name.InModelToEntity();
        var endpoint = $"/api/{modelName}/save-all";
        var content = new StringContent(JsonSerializer.Serialize(entities), Encoding.UTF8, "application/json");
        return await httpClient.PostAsync(endpoint, content);
    }

    public async Task DeleteAsync<T>(long id) where T : class
    {
        var modelName = typeof(T).Name.InModelToEntity();
        await httpClient.DeleteAsync($"/api/{modelName}/{id}");
    }

    public async Task DeleteAllAsync<T>(IEnumerable<long> ids) where T : class
    {
        var modelName = typeof(T).Name.InModelToEntity();
        var endpoint = $"/api/{modelName}/by-ids?ids={string.Join("&ids=", ids)}";
        await httpClient.DeleteAsync(endpoint);
    }

    public async Task<long> CountAsync<TIn, TOut>(IFindQuery<TIn>? query = null)
        where TIn : BaseInputModel
        where TOut : BaseOutputModel
    {
        var modelName = typeof(TOut).Name.OutModelToEntity();
        var endpoint = AppendFindQuery($"/api/{modelName}/count", query);
        var response = await httpClient.GetAsync(endpoint);
        if (!response.IsSuccessStatusCode)
        {
            return 0;
        }

        var content = await response.Content.ReadAsStringAsync();
        return long.TryParse(content, out var count) ? count : 0;
    }

    public async Task<TOut?> GetDataViewAsync<TOut>(string resource, long id, IDictionary<string, object?>? parameters = null)
        where TOut : class
    {
        var endpoint = $"/api/{resource}/{id}/data-view";
        if (parameters is { Count: > 0 })
        {
            var query = string.Join("&", parameters
                .Where(x => x.Value is not null)
                .Select(x => $"{x.Key}={Uri.EscapeDataString(Convert.ToString(x.Value, CultureInfo.InvariantCulture) ?? string.Empty)}"));
            if (!string.IsNullOrWhiteSpace(query))
            {
                endpoint += $"?{query}";
            }
        }

        var response = await httpClient.GetStringAsync(endpoint);
        if (string.IsNullOrWhiteSpace(response))
        {
            return null;
        }

        return JsonSerializer.Deserialize<TOut>(response, JsonHelper.OptionsDefault);
    }


    public async Task<OutAlbumList> FindAlbumsByArtistAsync(long artistId, IJoinSpecification<InAlbum>? joins = null, OrderSpecification<InAlbum>? order = null)
    {
        var endpoint = $"/api/albums/by-artist/{artistId}?{QueryStringHelper.SetUseCaseParameters(joins, order)}";
        var response = await httpClient.GetStringAsync(endpoint);
        return JsonSerializer.Deserialize<OutAlbumList>(response, JsonHelper.OptionsDefault) ?? new OutAlbumList { Items = [], Total = 0 };
    }

    public async Task<OutAlbumList> FindAlbumsByGenreAsync(long genreId, int genreOptions, IJoinSpecification<InAlbum>? joins = null, OrderSpecification<InAlbum>? order = null, PagingOptions? pagingOptions = null)
    {
        var endpoint = $"/api/albums/by-genre/{genreId}?genreOptions={genreOptions}&{QueryStringHelper.SetUseCaseParameters(joins, order, pagingOptions)}";
        var response = await httpClient.GetStringAsync(endpoint);
        return JsonSerializer.Deserialize<OutAlbumList>(response, JsonHelper.OptionsDefault) ?? new OutAlbumList { Items = [], Total = 0 };
    }

    public async Task<OutAlbumList> FindAlbumsByChartAsync(AlbumChartQuery query)
    {
        var response = await httpClient.GetStringAsync($"/api/albums/by-chart" + query.ToQueryString());
        return JsonSerializer.Deserialize<OutAlbumList>(response, JsonHelper.OptionsDefault) ?? new OutAlbumList();
    }

    public async Task<List<OutSong>> FindSongsByAlbumAsync(long albumId, IJoinSpecification<InSong>? joins = null, OrderSpecification<InSong>? order = null)
    {
        var endpoint = $"/api/songs/by-album/{albumId}?{QueryStringHelper.SetUseCaseParameters(joins, order)}";
        var response = await httpClient.GetStringAsync(endpoint);
        return JsonSerializer.Deserialize<List<OutSong>>(response, JsonHelper.OptionsDefault) ?? [];
    }
    
    public async Task<OutUserSongAttribute?> FindSongAttributeByUserAsync(long userId, long songId)
    {
        var endpoint = $"/api/user-song-attrs/by-user/{userId}/{songId}";
        _logger.LogInformation("🌍🏳️ GET {Endpoint}", endpoint);

        try
        {
            var response = await httpClient.GetStringAsync(endpoint);
            return JsonSerializer.Deserialize<OutUserSongAttribute>(response, JsonHelper.OptionsDefault);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "🌍⚠️ GET {Endpoint} failed", endpoint);
            return null;
        }
    }

    public async Task<OutGenericList<OutArtist>> FindArtistsByGenreAsync(long genreId, PagingOptions? pagingOptions = null)
    {
        var endpoint = $"/api/artists/by-genre/{genreId}?{QueryStringHelper.SetUseCaseParameters<InArtist>(pagingOptions: pagingOptions)}";
        var response = await httpClient.GetStringAsync(endpoint);
        return JsonSerializer.Deserialize<OutGenericList<OutArtist>>(response, JsonHelper.OptionsDefault) ?? new OutGenericList<OutArtist> { Items = [], Total = 0 };
    }

    private static string AppendFindQuery<TIn>(string endpoint, IFindQuery<TIn>? query) where TIn : BaseInputModel
    {
        if (query is null)
        {
            return endpoint;
        }

        var separator = endpoint.Contains('?') ? '&' : '?';
        var parts = new List<string>();

        if (query.RawSearch?.IsSet == true)
        {
            parts.Add($"filter={Uri.EscapeDataString(query.RawSearch.Value ?? string.Empty)}");
        }

        if (query.Search is not null)
        {
            parts.Add($"filterExact={query.Search.Exact.ToString().ToLowerInvariant()}");
            parts.Add($"filterSimilitude={query.Search.Similarity.ToString(CultureInfo.InvariantCulture)}");
        }
        
        if (query.GetType().Name == "ArtistFindQuery")
        {
            object? Read(string name) => query.GetType().GetProperty(name)?.GetValue(query);

            if (Read("MinRating") is decimal minRating) parts.Add($"minRating={minRating.ToString(CultureInfo.InvariantCulture)}");
            if (Read("MaxRating") is decimal maxRating) parts.Add($"maxRating={maxRating.ToString(CultureInfo.InvariantCulture)}");
            if (Read("MinUserAge") is short minUserAge) parts.Add($"minUserAge={minUserAge}");
            if (Read("MaxUserAge") is short maxUserAge) parts.Add($"maxUserAge={maxUserAge}");
            if (Read("PopularityWeight") is short popularityWeight) parts.Add($"popularityWeight={popularityWeight}");
            if (Read("ChartType") is Enum chartType) parts.Add($"chartType={Convert.ToInt32(chartType)}");
            if (Read("Discriminator") is Enum discriminator) parts.Add($"discriminator={Convert.ToInt32(discriminator)}");
            if (Read("Country") is string country && !string.IsNullOrWhiteSpace(country)) parts.Add($"country={Uri.EscapeDataString(country)}");
            if (Read("MainGenreId") is long mainGenreId) parts.Add($"mainGenreId={mainGenreId}");
            if (Read("InfluenceGenreIds") is IEnumerable<long> influenceGenreIds)
            {
                parts.AddRange(influenceGenreIds.Select(id => $"influenceGenreIds={id}"));
            }
        }

        if (parts.Count == 0)
        {
            return endpoint;
        }

        return endpoint + separator + string.Join("&", parts);
    }
}