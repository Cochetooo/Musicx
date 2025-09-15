using Microsoft.AspNetCore.Mvc;
using Musicx.Application.Api.Interfaces.Persistence;
using Musicx.Application.Api.Interfaces.Specifications;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Presentation.Web.Contexts;

namespace Musicx.Presentation.Web.Controllers;

[ApiController]
[Route("api/albums")]
public sealed class AlbumController(IAlbumRepository albumRepository,
    ILoggerProvider loggerProvider) : ControllerBase
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(AlbumController));
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] long id, [FromServices] IUserContext userContext)
    {
        _logger.LogInformation($"🌍🏳️ API : DELETE albums ({id})");

        if (false == userContext.Can("album.delete"))
        {
            _logger.LogInformation($"🌍⛔ API : DELETE albums ({id}) : NOT AUTHORIZED");
            return Unauthorized("Not authorized to delete albums.");
        }

        try
        {
            await albumRepository.DeleteAsync(id);
                    
            _logger.LogInformation($"🌍✅ API : DELETE albums ({id}) - SUCCESS");
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpDelete("by-ids")]
    public async Task<IActionResult> DeleteAll([FromRoute] long[] ids, [FromServices] IUserContext userContext)
    {
        var stringIds = string.Join(",", ids);
        _logger.LogInformation($"🌍🏳️ API : DELETE ALL albums ({stringIds})");
        
        if (false == userContext.Can("album.delete_all"))
        {
            _logger.LogInformation($"🌍⛔ API : DELETE ALL albums ({stringIds}) : NOT AUTHORIZED");
            return Unauthorized("Not authorized to delete albums.");
        }

        try
        {
            await albumRepository.DeleteAllAsync(ids);
                    
            _logger.LogInformation($"🌍✅ API : DELETE ALL albums ({stringIds}) - SUCCESS");
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OutAlbum>> FindById([FromRoute] long id, [FromQuery] string query = "")
    {
        _logger.LogInformation($"🌍🏳️ API : FIND BY ID albums ({id} & includes = {query})");

        try
        {
            var querySpecification = new AlbumQuerySpecification
            {
                IncludeArtist = query.Contains("artist"),
                IncludePrimaryGenres = query.Contains("genre"),
                IncludeInfluenceGenres = query.Contains("genre"),
                IncludeReleases = query.Contains("release")
            };
            
            var album = await albumRepository.FindByIdAsync(id, querySpecification);

            if (null == album)
            {
                _logger.LogInformation($"🌍❔ API : FIND BY ID albums ({id}) - NOT FOUND");
                return NoContent();
            }
            
            _logger.LogInformation($"🌍✅ API : FIND BY ID albums ({id}) - SUCCESS");
            return Ok(album);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpGet("by-artist/{artistId}")]
    public async Task<ActionResult<OutAlbum>> FindByArtistId([FromRoute] long artistId, [FromQuery] string query = "")
    {
        _logger.LogInformation($"🌍🏳️ API : FIND BY ARTIST albums ({artistId} & includes = {query})");

        try
        {
            var querySpecification = new AlbumQuerySpecification
            {
                IncludeArtist = query.Contains("artist"),
                IncludePrimaryGenres = query.Contains("genre"),
                IncludeInfluenceGenres = query.Contains("genre"),
                IncludeReleases = query.Contains("release")
            };
            
            var albums = await albumRepository.FindByArtistIdAsync(artistId, querySpecification);

            if (0 == albums.Count)
            {
                _logger.LogInformation($"🌍❔ API : FIND BY ARTIST albums ({artistId}) - NOT FOUND");
                return NoContent();
            }
            
            _logger.LogInformation($"🌍✅ API : FIND BY ARTIST albums ({artistId}) - SUCCESS");
            return Ok(albums);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpGet("by-genre/{genreId}")]
    public async Task<ActionResult<OutAlbum>> FindByGenreId([FromRoute] long genreId, 
        [FromQuery] int genreOptions,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 100,
        [FromQuery] string query = "")
    {
        _logger.LogInformation($"🌍🏳️ API : FIND BY GENRE albums ({genreId} " +
                               $"& genreOptions = {genreOptions} & includes = {query})");

        try
        {
            var querySpecification = new AlbumQuerySpecification
            {
                IncludeArtist = query.Contains("artist"),
                IncludePrimaryGenres = query.Contains("genre"),
                IncludeInfluenceGenres = query.Contains("genre"),
                IncludeReleases = query.Contains("release")
            };
            
            var albums = await albumRepository.FindByGenreIdAsync(
                genreId, 
                genreOptions, 
                skip, 
                take, 
                querySpecification
            );

            if (0 == albums.Count)
            {
                _logger.LogInformation($"🌍❔ API : FIND BY GENRE albums ({genreId}) - NOT FOUND");
                return NoContent();
            }
            
            _logger.LogInformation($"🌍✅ API : FIND BY GENRE albums ({genreId}) - SUCCESS");
            return Ok(albums);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OutAlbum>>> Find(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 100, 
        [FromQuery] string filter = "",
        [FromQuery] string query = "")
    {
        _logger.LogInformation($"🌍🏳️ API : FIND albums");

        try
        {
            var querySpecification = new AlbumQuerySpecification
            {
                IncludeArtist = query.Contains("artist"),
                IncludePrimaryGenres = query.Contains("genre"),
                IncludeInfluenceGenres = query.Contains("genre"),
                IncludeReleases = query.Contains("release")
            };
            
            var albums = await albumRepository.FindAsync(skip, take, querySpecification, filter);
            
            if (0 == albums.Count)
            {
                _logger.LogInformation($"🌍❔ API : FIND albums - NOT FOUND");
                return NoContent();
            }
            
            _logger.LogInformation($"🌍✅ API : FIND albums - SUCCESS");
            return Ok(albums);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpGet("by-ids")]
    public async Task<ActionResult<IEnumerable<OutArtist>>> FindIn(
        [FromQuery] long[] ids,
        [FromQuery] string query = "")
    {
        var stringIds = string.Join(',', ids);
        _logger.LogInformation($"🌍🏳️ API : FIND BY ID albums ({stringIds})");
        
        if (ids.Length == 0)
        {
            return BadRequest("❌ You must provide at least one ID.");
        }

        try
        {
            var querySpecification = new AlbumQuerySpecification
            {
                IncludeArtist = query.Contains("artist"),
                IncludePrimaryGenres = query.Contains("genre"),
                IncludeInfluenceGenres = query.Contains("genre"),
                IncludeReleases = query.Contains("release")
            };
            
            var albums = await albumRepository.FindIn(ids, querySpecification);
            
            _logger.LogInformation($"🌍✅ API : FIND BY ID albums ({stringIds}) - SUCCESS");
            return Ok(albums);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpGet("count")]
    public async Task<ActionResult<int>> GetCount()
    {
        _logger.LogInformation($"🌍🏳️ API : COUNT albums");
        
        try
        {
            var count = await albumRepository.GetCountAsync();
            
            _logger.LogInformation($"🌍✅ API : COUNT albums - SUCCESS");
            return Ok(count);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] InAlbum albumDto, [FromServices] IUserContext userContext)
    {
        _logger.LogInformation($"🌍🏳️ API : SAVE albums");
        
        if (false == userContext.Can("album.save"))
        {
            _logger.LogInformation($"🌍⛔ API : SAVE albums : NOT AUTHORIZED");
            return Unauthorized("Not authorized to save albums.");
        }

        try
        {
            await albumRepository.SaveAsync(albumDto);

            _logger.LogInformation($"🌍✅ API : SAVE albums - SUCCESS");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ API : SAVE albums - ERROR: {ex.Message}");
            return BadRequest(ex);
        }
    }
    
    [HttpPost("save-all")]
    public async Task<IActionResult> SaveAll([FromBody] IEnumerable<InAlbum> albumsDto,
        [FromServices] IUserContext userContext)
    {
        _logger.LogInformation($"🌍🏳️ API : SAVE ALL albums");
        
        if (false == userContext.Can("album.save_all"))
        {
            _logger.LogInformation($"🌍⛔ API : SAVE ALL albums : NOT AUTHORIZED");
            return Unauthorized("Not authorized to save albums.");
        }

        try
        {
            await albumRepository.SaveAllAsync(albumsDto);

            _logger.LogInformation($"🌍✅ API : SAVE ALL albums - SUCCESS");
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
}