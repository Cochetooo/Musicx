using Microsoft.AspNetCore.Mvc;
using Musicx.Application.Api.Interfaces.Persistence;
using Musicx.Application.Desktop.Specifications;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Presentation.Web.Contexts;

namespace Musicx.Presentation.Web.Controllers;

[ApiController]
[Route("api/songs")]
public sealed class SongController(ISongRepository songRepository,
    ILoggerProvider loggerProvider) : ControllerBase
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(SongController));
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] long id,
        [FromServices] IUserContext userContext)
    {
        _logger.LogInformation($"🌍🏳️ API : DELETE songs ({id})");
        
        if (false == userContext.Can("song.delete"))
        {
            _logger.LogInformation($"🌍⛔ API : DELETE songs : NOT AUTHORIZED");
            return Unauthorized("Not authorized to delete songs.");
        }

        try
        {
            await songRepository.DeleteAsync(id);
                    
            _logger.LogInformation($"🌍✅ API : DELETE songs ({id}) - SUCCESS");
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
        _logger.LogInformation($"🌍🏳️ API : DELETE ALL songs ({stringIds})");
        
        if (false == userContext.Can("song.delete_all"))
        {
            _logger.LogInformation($"🌍⛔ API : DELETE ALL songs : NOT AUTHORIZED");
            return Unauthorized("Not authorized to delete all songs.");
        }

        try
        {
            await songRepository.DeleteAllAsync(ids);
                    
            _logger.LogInformation($"🌍✅ API : DELETE ALL songs ({stringIds}) - SUCCESS");
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OutSong>> FindById([FromRoute] long id, [FromQuery] string query = "")
    {
        _logger.LogInformation($"🌍🏳️ API : FIND BY ID songs ({id} & includes = {query})");

        try
        {
            var querySpecification = new SongQuerySpecification
            {
                IncludeArtist = query.Contains("artist"),
                IncludeAlbum = query.Contains("album"),
                IncludeAlbumArtist = query.Contains("alart"),
                IncludePrimaryGenres = query.Contains("genre"),
                IncludeInfluenceGenres = query.Contains("genre"),
            };
            
            var album = await songRepository.FindByIdAsync(id, querySpecification);

            if (null == album)
            {
                _logger.LogInformation($"🌍❔ API : FIND BY ID songs ({id}) - NOT FOUND");
                return NoContent();
            }
            
            _logger.LogInformation($"🌍✅ API : FIND BY ID songs ({id}) - SUCCESS");
            return Ok(album);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpGet("by-album/{albumId}")]
    public async Task<ActionResult<OutSong>> FindByAlbumId([FromRoute] long albumId, [FromQuery] string query = "")
    {
        _logger.LogInformation($"🌍🏳️ API : FIND BY ALBUM songs ({albumId} & includes = {query})");

        try
        {
            var querySpecification = new SongQuerySpecification
            {
                IncludeArtist = query.Contains("artist"),
                IncludeAlbum = query.Contains("album"),
                IncludeAlbumArtist = query.Contains("alart"),
                IncludePrimaryGenres = query.Contains("genre"),
                IncludeInfluenceGenres = query.Contains("genre"),
            };
            
            var songs = await songRepository.FindByAlbumIdAsync(albumId, querySpecification);

            if (0 == songs.Count)
            {
                _logger.LogInformation($"🌍❔ API : FIND BY ALBUM songs ({albumId}) - NOT FOUND");
                return NoContent();
            }
            
            _logger.LogInformation($"🌍✅ API : FIND BY ALBUM songs ({albumId}) - SUCCESS");
            return Ok(songs);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OutSong>>> Find(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 100, 
        [FromQuery] string filter = "",
        [FromQuery] string query = "")
    {
        _logger.LogInformation($"🌍🏳️ API : FIND songs");

        try
        {
            var querySpecification = new SongQuerySpecification
            {
                IncludeArtist = query.Contains("artist"),
                IncludeAlbum = query.Contains("album"),
                IncludeAlbumArtist = query.Contains("alart"),
                IncludePrimaryGenres = query.Contains("genre"),
                IncludeInfluenceGenres = query.Contains("genre"),
            };
            
            var songs = await songRepository.FindAsync(skip, take, querySpecification, filter);
            
            if (0 == songs.Count)
            {
                _logger.LogInformation($"🌍❔ API : FIND songs - NOT FOUND");
                return NoContent();
            }
            
            _logger.LogInformation($"🌍✅ API : FIND songs - SUCCESS");
            return Ok(songs);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpGet("by-ids")]
    public async Task<ActionResult<IEnumerable<OutSong>>> FindIn(
        [FromQuery] long[] ids,
        [FromQuery] string query = "")
    {
        var stringIds = string.Join(',', ids);
        _logger.LogInformation($"🌍🏳️ API : FIND BY ID songs ({stringIds})");
        
        if (ids.Length == 0)
        {
            return BadRequest("❌ You must provide at least one ID.");
        }

        try
        {
            var querySpecification = new SongQuerySpecification
            {
                IncludeArtist = query.Contains("artist"),
                IncludeAlbum = query.Contains("album"),
                IncludeAlbumArtist = query.Contains("alart"),
                IncludePrimaryGenres = query.Contains("genre"),
                IncludeInfluenceGenres = query.Contains("genre"),
            };
            
            var songs = await songRepository.FindIn(ids, querySpecification);
            
            _logger.LogInformation($"🌍✅ API : FIND BY ID songs ({stringIds}) - SUCCESS");
            return Ok(songs);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpGet("count")]
    public async Task<ActionResult<long>> GetCount()
    {
        _logger.LogInformation($"🌍🏳️ API : COUNT songs");
        
        try
        {
            var count = await songRepository.GetCountAsync();
            
            _logger.LogInformation($"🌍✅ API : COUNT songs - SUCCESS");
            return Ok(count);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] InSong songDto,
        [FromServices] IUserContext userContext)
    {
        _logger.LogInformation($"🌍🏳️ API : SAVE songs");
        
        if (false == userContext.Can("song.save"))
        {
            _logger.LogInformation($"🌍⛔ API : SAVE songs : NOT AUTHORIZED");
            return Unauthorized("Not authorized to save songs.");
        }

        try
        {
            await songRepository.SaveAsync(songDto);

            _logger.LogInformation($"🌍✅ API : SAVE songs - SUCCESS");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ API : SAVE songs - ERROR: {ex.Message}");
            return BadRequest(ex);
        }
    }
    
    [HttpPost("save-all")]
    public async Task<IActionResult> SaveAll([FromBody] IEnumerable<InSong> songsDto,
        [FromServices] IUserContext userContext)
    {
        _logger.LogInformation($"🌍🏳️ API : SAVE ALL songs");
        
        if (false == userContext.Can("song.save_all"))
        {
            _logger.LogInformation($"🌍⛔ API : SAVE ALL songs : NOT AUTHORIZED");
            return Unauthorized("Not authorized to save songs.");
        }

        try
        {
            await songRepository.SaveAllAsync(songsDto);

            _logger.LogInformation($"🌍✅ API : SAVE ALL songs - SUCCESS");
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
}