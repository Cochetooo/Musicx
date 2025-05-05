using Microsoft.AspNetCore.Mvc;
using Musicx.Application.Api.Interfaces.Persistence;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Mappers;
using ILoggerFactory = Musicx.Application.Shared.Interfaces.Common.ILoggerFactory;

namespace Musicx.Presentation.Api.Controllers;

[ApiController]
[Route("api/artists")]
public sealed class ArtistController(
    ILoggerFactory loggerFactory,
    IArtistRepository artistRepository) : ControllerBase
{
    private readonly Application.Shared.Interfaces.Common.ILogger<ArtistController> _logger = loggerFactory.CreateLogger<ArtistController>();
    
    [HttpGet("{id:long}")]
    public async Task<ActionResult<OutArtist>> GetArtistByIdAsync(long id)
    {
        _logger.Debug($"⛏️ Get Artist By Id : {id}");

        var artist = await artistRepository.FindByIdAsync(id);

        if (null == artist)
        {
            return NotFound();
        }

        return Ok(artist.ToDto());
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OutArtist>>> GetArtistsAsync(int skip = 0, int take = 200, string? filter = null) 
    {
        _logger.Debug($"⛏️ Get All Artists : Skip: {skip}, Take: {take}");
        
        var artists = await artistRepository.FindAsync(skip, take, g => string.IsNullOrEmpty(filter) || g.Name.Contains(filter));

        var response = artists.Select(a => a.ToDto()).ToList();

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> SaveAsync([FromBody] InArtist? artist)
    {
        if (null == artist)
        {
            return BadRequest("Invalid Data : null");
        }

        await artistRepository.SaveAsync(artist.ToEntity());

        return Ok();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteAsync(long id)
    {
        await artistRepository.DeleteAsync(id);
        return NoContent();
    }
}