using Microsoft.AspNetCore.Mvc;
using Musicx.Application.Api.Interfaces.Storage;
using Musicx.Application.Shared.Interfaces.Providers.ExternalMusicData;
using Musicx.Application.Shared.Interfaces.UseCases.ExternalMusicData;
using Musicx.Contracts.Dto.Responses.Specifics.Artwork;

namespace Musicx.Presentation.Web.Controllers.Artwork;

[ApiController]
[Route("api/artworks")]
public sealed class ArtworkController(
    IFetchAlbumInfoClientService fetchAlbumInfoClientService,
    IFetchArtistInfoClientService fetchArtistInfoClientService,
    IArtworkStorage artworkStorage,
    HttpClient httpClient,
    ILoggerProvider loggerProvider) : ControllerBase
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(ArtworkController));

    [HttpGet("artist-options")]
    public async Task<ActionResult<OutArtworkSearchResult>> GetArtistOptionsAsync([FromQuery] string name, CancellationToken token)
    {
        _logger.LogInformation("🌍🏳️ API : GET artist artwork options ({Name})", name);
        return Ok(await fetchArtistInfoClientService.ExecuteAsync(new FetchArtistInfoRequest(name, token)));
    }

    [HttpGet("album-options")]
    public async Task<ActionResult<OutArtworkSearchResult>> GetAlbumOptionsAsync([FromQuery] string name, [FromQuery] string artist, CancellationToken token)
    {
        _logger.LogInformation("🌍🏳️ API : GET album artwork options ({Artist} / {Name})", artist, name);
        return Ok(await fetchAlbumInfoClientService.ExecuteAsync(new FetchAlbumInfoRequest(name, artist, token)));
    }

    [HttpPost("artists/{artistId:long}")]
    [RequestSizeLimit(10_000_000)]
    public async Task<ActionResult<string>> SaveArtistArtworkAsync([FromRoute] long artistId, [FromForm] IFormFile? file, [FromForm] string? remoteUrl, CancellationToken token)
    {
        var url = await SaveAsync(streamFactory: ct => OpenStreamAsync(file, remoteUrl, ct), save: (stream, contentType, ct) => artworkStorage.SaveArtistAsync(artistId, stream, contentType, ct), token);
        return Ok(url);
    }

    [HttpPost("artists/{artistId:long}/albums/{albumId:long}")]
    [RequestSizeLimit(10_000_000)]
    public async Task<ActionResult<string>> SaveAlbumArtworkAsync([FromRoute] long artistId, [FromRoute] long albumId, [FromForm] IFormFile? file, [FromForm] string? remoteUrl, CancellationToken token)
    {
        var url = await SaveAsync(streamFactory: ct => OpenStreamAsync(file, remoteUrl, ct), save: (stream, contentType, ct) => artworkStorage.SaveAlbumAsync(artistId, albumId, stream, contentType, ct), token);
        return Ok(url);
    }

    private async Task<string> SaveAsync(Func<CancellationToken, Task<(Stream Stream, string ContentType)>> streamFactory, Func<Stream, string, CancellationToken, Task<string>> save, CancellationToken token)
    {
        var (stream, contentType) = await streamFactory(token);
        await using (stream)
        {
            return await save(stream, contentType, token);
        }
    }

    private async Task<(Stream Stream, string ContentType)> OpenStreamAsync(IFormFile? file, string? remoteUrl, CancellationToken token)
    {
        if (file is not null)
        {
            return (file.OpenReadStream(), file.ContentType);
        }

        if (string.IsNullOrWhiteSpace(remoteUrl) || !Uri.TryCreate(remoteUrl, UriKind.Absolute, out var uri))
        {
            throw new BadHttpRequestException("You must provide a valid file or remoteUrl.");
        }

        if (!string.Equals(uri.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase)
            && !string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
        {
            throw new BadHttpRequestException("Only http(s) remote artwork urls are supported.");
        }

        var response = await httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, token);
        response.EnsureSuccessStatusCode();
        var stream = await response.Content.ReadAsStreamAsync(token);
        var contentType = response.Content.Headers.ContentType?.MediaType ?? "image/webp";
        return (stream, contentType);
    }
}