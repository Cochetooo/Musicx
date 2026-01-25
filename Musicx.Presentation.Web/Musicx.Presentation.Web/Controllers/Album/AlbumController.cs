using Microsoft.AspNetCore.Mvc;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.Album;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Helpers;
using Musicx.Contracts.Dto.Requests.Album;
using Musicx.Contracts.Dto.Requests.Specifics;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Infrastructure.API.Persistence.Specifications.Album;
using Musicx.Presentation.Web.Contexts;

namespace Musicx.Presentation.Web.Controllers.Album;

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
    public async Task<ActionResult<OutAlbum>> FindById([FromRoute] long id, 
        [FromQuery] AlbumJoinSpecification? joins = null)
    {
        _logger.LogInformation($"🌍🏳️ API : FIND BY ID albums ({id} & includes = {joins})");

        try
        {
            var album = await albumRepository.FindOneByIdAsync(id, joins);

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
    public async Task<ActionResult<OutAlbumList>> FindByArtistId(
        [FromRoute] long artistId, 
        [FromQuery] AlbumJoinSpecification? joins = null,
        [FromQuery] AlbumOrderSpecification? order = null)
    {
        _logger.LogInformation($"🌍🏳️ API : FIND BY ARTIST albums ({artistId})");

        try
        {
            var albums = await albumRepository.FindByArtistIdAsync(
                artistId, 
                joins,
                order
            );

            if (0 == albums.Count)
            {
                _logger.LogInformation($"🌍❔ API : FIND BY ARTIST albums ({artistId}) - NOT FOUND");
                return NoContent();
            }
            
            var response = new OutAlbumList
            {
                AverageRating = RatingHelper.CalculateArtistRating(albums),
                Items = albums,
                Total = albums.Count
            };
            
            _logger.LogInformation($"🌍✅ API : FIND BY ARTIST albums ({artistId}) - SUCCESS");
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpGet("by-chart")]
    public async Task<ActionResult<OutAlbumList>> FindByChart([FromQuery] AlbumChartQuery query)
    {
        _logger.LogInformation($"🌍🏳️ API : FIND BY CHART albums");

        try
        {
            var albums = await albumRepository.FindByChart(query);

            if (0 == albums.Count)
            {
                _logger.LogInformation($"🌍❔ API : FIND BY CHART albums - NOT FOUND");
                return NoContent();
            }
            
            var response = new OutAlbumList
            {
                AverageRating = RatingHelper.CalculateArtistRating(albums),
                Items = albums,
                Total = albums.Count
            };
            
            _logger.LogInformation($"🌍✅ API : FIND BY CHART albums - SUCCESS");
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpGet("by-genre/{genreId}")]
    public async Task<ActionResult<OutAlbumList>> FindByGenreId([FromRoute] long genreId, 
        [FromQuery] int genreOptions,
        [FromQuery] AlbumJoinSpecification? joins = null,
        [FromQuery] AlbumOrderSpecification? order = null,
        [FromQuery] PagingOptions? paging = null)
    {
        _logger.LogInformation($"🌍🏳️ API : FIND BY GENRE albums ({genreId} " +
                               $"& genreOptions = {genreOptions})");

        try
        {
            var albums = await albumRepository.FindByGenreIdAsync(
                genreId, 
                genreOptions, 
                joins,
                order,
                paging
            );

            if (0 == albums.Count)
            {
                _logger.LogInformation($"🌍❔ API : FIND BY GENRE albums ({genreId}) - NOT FOUND");
                return NoContent();
            }

            var albumCount = await albumRepository.GetCountByGenreIdAsync(genreId);

            var response = new OutAlbumList
            {
                AverageRating = RatingHelper.CalculateArtistRating(albums),
                Items = albums,
                Total = albumCount
            };
            
            _logger.LogInformation($"🌍✅ API : FIND BY GENRE albums ({genreId}) - SUCCESS");
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpGet]
    public async Task<ActionResult<OutAlbumList>> Find(
        [FromQuery] bool filterExact = false,
        [FromQuery] double filterSimilitude = 0.4,
        [FromQuery] string filter = "",
        [FromQuery] AlbumJoinSpecification? joins = null,
        [FromQuery] AlbumOrderSpecification? order = null,
        [FromQuery] PagingOptions? paging = null)
    {
        _logger.LogInformation($"🌍🏳️ API : FIND albums | filter = {filter}");

        if (joins is not null)
        {
            _logger.LogDebug(joins.ToString());
        }

        _logger.LogDebug("Order null: " + (order is null));

        try
        {
            var albums = await albumRepository.FindAllAsync(
                filterExact,
                filterSimilitude, 
                filter,
                joins,
                order,
                paging
            );
            
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
        [FromQuery] AlbumJoinSpecification? joins = null,
        [FromQuery] AlbumOrderSpecification? order = null)
    {
        var stringIds = string.Join(',', ids);
        _logger.LogInformation($"🌍🏳️ API : FIND BY ID albums ({stringIds})");
        
        if (ids.Length == 0)
        {
            return BadRequest("❌ You must provide at least one ID.");
        }

        try
        {
            var albums = await albumRepository.FindInAsync(ids, joins, order);
            
            _logger.LogInformation($"🌍✅ API : FIND BY ID albums ({stringIds}) - SUCCESS");
            return Ok(albums);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpGet("count")]
    public async Task<ActionResult<long>> GetCount()
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
            var result = await albumRepository.SaveAsync(albumDto);

            _logger.LogInformation($"🌍✅ API : SAVE albums - SUCCESS");
            return Ok(result);
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