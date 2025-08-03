using System.Text.Json;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Utilities;
using Musicx.Application.Web.Interfaces.UseCases.Specifics;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Infrastructure.Web.UseCases.Specifics;

public sealed class UcGetAlbumByGenre(
    HttpClient httpClient,
    ILoggerProvider loggerProvider) : IGetAlbumByGenreUseCase
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(UcGetAlbumByGenre));

    public async Task<List<OutAlbum>> ExecuteAsync(long genreId, 
        int genreOptions,
        int skip = 0,
        int take = 100,
        string query = "")
    {
        var endpoint = $"/api/albums/by-genre/{genreId}?" +
                       $"genreOptions={genreOptions}&skip={skip}&take={take}&query={query}";
        
        _logger.LogInformation("🌍🏳️ GET " + endpoint);
        
        var response = await httpClient.GetStringAsync(endpoint);

        if (string.IsNullOrWhiteSpace(response))
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Null response");
            return [];
        }
        
        var json = JsonSerializer.Deserialize<List<OutAlbum>>(response, JsonHelper.OptionsDefault);

        if (null == json)
        {
            _logger.LogWarning($"🌍⚠️ GET {endpoint} - WARNING : Null response");
            return [];
        }
        
        _logger.LogInformation($"🌍✅ GET {endpoint} - SUCCESS");

        return json;
    }
    
    public async Task<List<OutAlbum>> ExecuteAsync(OutGenre genre, int genreOptions,
        int skip = 0,
        int take = 100,
        string query = "")
        => await ExecuteAsync(genre.Id, genreOptions, skip, take, query);

    public List<OutAlbum> Execute(long genreId, int genreOptions,
        int skip = 0,
        int take = 100,
        string query = "")
    {
        throw new NotImplementedException();
    }

    public List<OutAlbum> Execute(OutGenre genre, int genreOptions,
        int skip = 0,
        int take = 100,
        string query = "")
        => Execute(genre.Id, genreOptions, skip, take, query);
}