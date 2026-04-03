using Microsoft.AspNetCore.Mvc;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.Song;
using Musicx.Application.API.Persistence.Queries;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Helpers;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Song;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Infrastructure.API.Persistence.Specifications.Song;
using Musicx.Presentation.Web.Contexts;

namespace Musicx.Presentation.Web.Controllers.Song;

[ApiController]
[Route("api/songs")]
public sealed class SongController(ISongRepository songRepository,
    ILoggerProvider loggerProvider) : ControllerBase
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(SongController));
    
    private static string? ValidateSongTitle(string? title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return null;
        }

        if (EnglishTitleCaseHelper.IsValid(title))
        {
            return null;
        }

        var expected = EnglishTitleCaseHelper.ToTitleCase(title);
        return $"Song title must use English title case. Expected: '{expected}'.";
    }
    
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
    public async Task<ActionResult<OutSong>> FindById([FromRoute] long id, 
        [FromQuery] SongJoinSpecification? joins = null)
    {
        _logger.LogInformation($"🌍🏳️ API : FIND BY ID songs ({id})");

        try
        {
            var album = await songRepository.FindOneByIdAsync(id, joins);

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
    public async Task<ActionResult<OutSong>> FindByAlbumId([FromRoute] long albumId, 
        [FromQuery] SongJoinSpecification? joins = null,
        [FromQuery] SongOrderSpecification? order = null)
    {
        _logger.LogInformation($"🌍🏳️ API : FIND BY ALBUM songs ({albumId})");

        try
        {
            var songs = await songRepository.FindByAlbumIdAsync(albumId, joins, order);

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
    public async Task<ActionResult<OutGenericList<OutSong>>> Find(
        [FromQuery] bool filterExact = false,
        [FromQuery] double filterSimilitude = 0.4,
        [FromQuery] string filter = "",
        [FromQuery] SongJoinSpecification? joins = null,
        [FromQuery] SongOrderSpecification? order = null,
        [FromQuery] PagingOptions? paging = null)
    {
        _logger.LogInformation($"🌍🏳️ API : FIND songs | filter = {filter}");

        try
        {
            var songs = await songRepository.FindAsync(
                new SongFindQuery
                {
                    Search = new() { Exact = filterExact, Similarity = filterSimilitude },
                    RawSearch = string.IsNullOrWhiteSpace(filter) ? null : new(filter)
                },
                joins, 
                order,
                paging
            );
            
            if (0 == songs.Total)
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
        [FromQuery] SongJoinSpecification? joins = null,
        [FromQuery] SongOrderSpecification? order = null)
    {
        var stringIds = string.Join(',', ids);
        _logger.LogInformation($"🌍🏳️ API : FIND BY ID songs ({stringIds})");
        
        if (ids.Length == 0)
        {
            return BadRequest("❌ You must provide at least one ID.");
        }

        try
        {
            var songs = await songRepository.FindInAsync(ids, joins, order);
            
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
            var count = await songRepository.CountAsync();
            
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
        
        var validationError = ValidateSongTitle(songDto.Title);
        if (validationError is not null)
        {
            return BadRequest(validationError);
        }

        try
        {
            var result = await songRepository.SaveAsync(songDto);

            _logger.LogInformation($"🌍✅ API : SAVE songs - SUCCESS");
            return Ok(result);
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
        
        var validationError = songsDto
            .Select(s => ValidateSongTitle(s.Title))
            .FirstOrDefault(err => err is not null);

        if (validationError is not null)
        {
            return BadRequest(validationError);
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