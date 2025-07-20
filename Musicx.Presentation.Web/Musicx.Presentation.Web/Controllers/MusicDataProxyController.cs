using Microsoft.AspNetCore.Mvc;
using Musicx.Application.Shared.Interfaces.UseCases.ExternalMusicData;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Presentation.Web.Controllers;

[ApiController]
[Route("api/external")]
public sealed class MusicDataProxyController(
    ILoggerProvider loggerProvider,
    IFetchArtistInfoUseCase ucFetchArtistInfo,
    IFetchAlbumInfoUseCase ucFetchAlbumInfo) : ControllerBase
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(MusicDataProxyController));
    
    [HttpGet("artist")]
    public async Task<ActionResult<OutArtist?>> GetArtistInfoAsync(string name, 
        CancellationToken ct = default)
    {
        _logger.LogInformation($"🌍🏳️ API : GET external/artist ({name})");
        
        var response = await ucFetchArtistInfo.ExecuteAsync(new FetchArtistInfoRequest
        (
            Name: name,
            CancellationToken: ct
        ));
        
        _logger.LogInformation($"🌍✅ API : GET external/artist ({name}) - SUCCESS");

        return Ok(response.Artist);
    }
    
    [HttpGet("album")]
    public async Task<ActionResult<OutAlbum?>> GetAlbumInfoAsync(string name, 
        string artist, CancellationToken ct = default)
    {
        _logger.LogInformation($"🌍🏳️ API : GET external/album ({name})");
        
        var response = await ucFetchAlbumInfo.ExecuteAsync(new FetchAlbumInfoRequest
        (
            Name: name,
            Artist: artist,
            CancellationToken: ct
        ));
        
        _logger.LogInformation($"🌍✅ API : GET external/album ({name}) - SUCCESS");

        return Ok(response.Album);
    }
}