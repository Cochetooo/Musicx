using Microsoft.AspNetCore.Mvc;
using Musicx.Application.Api.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Presentation.Web.Contexts;

namespace Musicx.Presentation.Web.Controllers;

[ApiController]
[Route("api/artists")]
public sealed class ArtistController(IArtistRepository artistRepository,
        ILoggerProvider loggerProvider) : ControllerBase
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(ArtistController));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] long id, 
        [FromServices] IUserContext userContext)
    {
        _logger.LogInformation($"🌍🏳️ API : DELETE artists ({id})");

        if (false == userContext.Can("artist.delete"))
        {
            _logger.LogInformation($"🌍⛔ API : DELETE artists : NOT AUTHORIZED");
            return Unauthorized("Not authorized to delete artists.");
        }

        try
        {
            await artistRepository.DeleteAsync(id);
                    
            _logger.LogInformation($"🌍✅ API : DELETE artists ({id}) - SUCCESS");
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpDelete("by-ids")]
    public async Task<IActionResult> DeleteAll([FromRoute] long[] ids,
        [FromServices] IUserContext userContext)
    {
        var stringIds = string.Join(",", ids);
        _logger.LogInformation($"🌍🏳️ API : DELETE ALL artists ({stringIds})");
        
        if (false == userContext.Can("artist.delete_all"))
        {
            _logger.LogInformation($"🌍⛔ API : DELETE ALL artists : NOT AUTHORIZED");
            return Unauthorized("Not authorized to delete all artists.");
        }

        try
        {
            await artistRepository.DeleteAllAsync(ids);
                    
            _logger.LogInformation($"🌍✅ API : DELETE ALL artists ({stringIds}) - SUCCESS");
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OutArtist>> FindById([FromRoute] long id)
    {
        _logger.LogInformation($"🌍🏳️ API : FIND BY ID artists ({id})");

        try
        {
            var artist = await artistRepository.FindByIdAsync(id);

            if (null == artist)
            {
                _logger.LogInformation($"🌍❔ API : FIND BY ID artists ({id}) - NOT FOUND");
                return NoContent();
            }
            
            _logger.LogInformation($"🌍✅ API : FIND BY ID artists ({id}) - SUCCESS");
            return Ok(artist);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OutArtist>>> Find(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 100, 
        [FromQuery] string filter = "")
    {
        _logger.LogInformation($"🌍🏳️ API : FIND artists");

        try
        {
            var artists = await artistRepository.FindAsync(
                skip: skip, 
                take: take,
                filter: filter
            );
            
            _logger.LogInformation($"🌍✅ API : FIND artists - SUCCESS");
            return Ok(artists);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpGet("by-ids")]
    public async Task<ActionResult<IEnumerable<OutArtist>>> FindIn([FromQuery] long[] ids)
    {
        var stringIds = string.Join(',', ids);
        _logger.LogInformation($"🌍🏳️ API : FIND BY ID artists ({stringIds})");
        
        if (ids.Length == 0)
        {
            return BadRequest("❌ You must provide at least one ID.");
        }

        try
        {
            var artists = await artistRepository.FindIn(ids);
            
            _logger.LogInformation($"🌍✅ API : FIND BY ID artists ({stringIds}) - SUCCESS");
            return Ok(artists);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpGet("count")]
    public async Task<ActionResult<long>> GetCount()
    {
        _logger.LogInformation($"🌍🏳️ API : COUNT artists");
        
        try
        {
            var count = await artistRepository.GetCountAsync();
            
            _logger.LogInformation($"🌍✅ API : COUNT artists - SUCCESS");
            return Ok(count);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] InArtist artistDto,
        [FromServices] IUserContext userContext)
    {
        _logger.LogInformation($"🌍🏳️ API : SAVE artists");
        
        if (false == userContext.Can("artist.save"))
        {
            _logger.LogInformation($"🌍⛔ API : SAVE artists : NOT AUTHORIZED");
            return Unauthorized("Not authorized to save artists.");
        }

        try
        {
            await artistRepository.SaveAsync(artistDto);

            _logger.LogInformation($"🌍✅ API : SAVE artists - SUCCESS");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ API : SAVE artists - ERROR: {ex.Message}");
            return BadRequest(ex);
        }
    }
    
    [HttpPost("save-all")]
    public async Task<IActionResult> SaveAll([FromBody] IEnumerable<InArtist> artistsDto,
        [FromServices] IUserContext userContext)
    {
        _logger.LogInformation($"🌍🏳️ API : SAVE ALL artists");
        
        if (false == userContext.Can("artist.save_all"))
        {
            _logger.LogInformation($"🌍⛔ API : SAVE ALL artists : NOT AUTHORIZED");
            return Unauthorized("Not authorized to save artists.");
        }

        try
        {
            await artistRepository.SaveAllAsync(artistsDto);

            _logger.LogInformation($"🌍✅ API : SAVE ALL artists - SUCCESS");
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
}