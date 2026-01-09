using System.Text.Json;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Helpers;
using Musicx.Application.Web.Interfaces.UseCases.Specifics;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;

namespace Musicx.Infrastructure.Web.UseCases.Specifics;

public sealed class UcGetAlbumByGenre(
    HttpClient httpClient,
    ILoggerFactory loggerProvider) : IGetAlbumByGenreUseCase
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(UcGetAlbumByGenre));

    public async Task<OutAlbumList> ExecuteAsync(long genreId, 
        int genreOptions,
        long skip = 0,
        long take = 100,
        string order = "",
        string query = "")
    {
        var endpoint = $"/api/albums/by-genre/{genreId}?" +
                       $"genreOptions={genreOptions}&skip={skip}&take={take}&order={order}&query={query}";
        
        _logger.LogInformation("🌍🏳️ GET " + endpoint);
        
        var response = await httpClient.GetStringAsync(endpoint);

        if (string.IsNullOrWhiteSpace(response))
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Null response");
            return new OutAlbumList
            {
                Items = [],
                Total = 0,
                AverageRating = null,
            };
        }
        
        var json = JsonSerializer.Deserialize<OutAlbumList>(response, JsonHelper.OptionsDefault);

        if (null == json)
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Null response");
            return new OutAlbumList
            {
                Items = [],
                Total = 0,
                AverageRating = null,
            };
        }
        
        _logger.LogInformation($"🌍✅ GET {endpoint} - SUCCESS");

        return json;
    }
    
    public async Task<OutAlbumList> ExecuteAsync(OutGenre genre, int genreOptions,
        long skip = 0,
        long take = 100,
        string order = "",
        string query = "")
        => await ExecuteAsync(genre.Id, genreOptions, skip, take, order, query);

    public OutAlbumList Execute(long genreId, int genreOptions,
        long skip = 0,
        long take = 100,
        string order = "",
        string query = "")
    {
        throw new NotImplementedException();
    }

    public OutAlbumList Execute(OutGenre genre, int genreOptions,
        long skip = 0,
        long take = 100,
        string order = "",
        string query = "")
        => Execute(genre.Id, genreOptions, skip, take, order, query);
}