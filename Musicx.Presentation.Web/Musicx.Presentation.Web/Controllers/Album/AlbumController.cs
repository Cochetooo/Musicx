using Microsoft.AspNetCore.Mvc;
using Musicx.Application.Api.Interfaces.DataViews;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.Album;
using Musicx.Application.API.Persistence.Queries;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Helpers;
using Musicx.Contracts.Dto.Requests.Album;
using Musicx.Contracts.Dto.Requests.Specifics;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Artist;
using Musicx.Contracts.Dto.Responses.Specifics.Albums;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Infrastructure.API.Persistence.Specifications.Album;
using Musicx.Presentation.Web.Contexts;

namespace Musicx.Presentation.Web.Controllers.Album;

[ApiController]
[Route("api/albums")]
public sealed class AlbumController(IAlbumRepository albumRepository,
    IAlbumDataViewBuilder albumDataViewBuilder,
    ILoggerProvider loggerProvider) : ControllerBase
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(AlbumController));
    
    private static string? ValidateAlbumName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        if (EnglishTitleCaseHelper.IsValid(name))
        {
            return null;
        }

        var expected = EnglishTitleCaseHelper.ToTitleCase(name);
        return $"Album name must use English title case. Expected: '{expected}'.";
    }
    
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
    
    [HttpGet("by-artist")]
    public async Task<ActionResult<Dictionary<long, OutAlbumList>>> FindByArtistIds(
        [FromQuery] long[] artistIds,
        [FromQuery] AlbumJoinSpecification? joins = null,
        [FromQuery] AlbumOrderSpecification? order = null)
    {
        var stringIds = string.Join(',', artistIds);
        _logger.LogInformation($"🌍🏳️ API : FIND BY ARTISTS albums ({stringIds})");

        if (artistIds.Length == 0)
        {
            return BadRequest("❌ You must provide at least one artist ID.");
        }

        try
        {
            var albumsByArtist = await albumRepository.FindByArtistIdsAsync(artistIds, joins, order);
            var response = albumsByArtist.ToDictionary(
                x => x.Key,
                x => new OutAlbumList
                {
                    Items = x.Value,
                    Total = x.Value.Count,
                    AverageRating = RatingHelper.CalculateArtistRating(x.Value)
                });

            if (response.Count == 0)
            {
                _logger.LogInformation($"🌍❔ API : FIND BY ARTISTS albums ({stringIds}) - NOT FOUND");
                return NoContent();
            }

            _logger.LogInformation($"🌍✅ API : FIND BY ARTISTS albums ({stringIds}) - SUCCESS");
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpGet("{id}/data-view")]
    public async Task<ActionResult<OutAlbumDataView>> FindDataView(
        [FromRoute] long id,
        [FromServices] IUserContext userContext,
        [FromQuery] long? userId = null)
    {
        _logger.LogInformation($"🌍🏳️ API : FIND DATA VIEW albums ({id})");

        try
        {
            var effectiveUserId = userId ?? userContext.CurrentUser?.Id;
            var view = await albumDataViewBuilder.BuildAsync(new AlbumDataViewQuery(id, effectiveUserId));

            if (view is null)
            {
                _logger.LogInformation($"🌍❔ API : FIND DATA VIEW albums ({id}) - NOT FOUND");
                return NoContent();
            }

            _logger.LogInformation($"🌍✅ API : FIND DATA VIEW albums ({id}) - SUCCESS");
            return Ok(view);
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
    
    [HttpGet("{id}/similar")]
    public async Task<ActionResult<OutGenericList<OutAlbum>>> FindSimilarAlbums(
        [FromRoute] long id,
        [FromQuery] AlbumOrderSpecification? order = null,
        [FromQuery] PagingOptions? paging = null)
    {
        _logger.LogInformation($"🌍🏳️ API : FIND SIMILAR albums ({id})");

        try
        {
            var result = await albumRepository.FindSimilarAsync(id, order, paging);

            if (0 == result.Total)
            {
                _logger.LogInformation($"🌍❔ API : FIND SIMILAR albums ({id}) - NOT FOUND");
                return NoContent();
            }

            _logger.LogInformation($"🌍✅ API : FIND SIMILAR albums ({id}) - SUCCESS");
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpGet]
    public async Task<ActionResult<OutGenericList<OutAlbum>>> Find(
        [FromQuery] bool filterExact = false,
        [FromQuery] double filterSimilitude = 0.4,
        [FromQuery] string filter = "",
        [FromQuery] DateTime? createdAtFrom = null,
        [FromQuery] DateTime? createdAtTo = null,
        [FromQuery] bool? isVisible = null,
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
            var albums = await albumRepository.FindAsync(
                new AlbumFindQuery
                {
                    Search = new() { Exact = filterExact, Similarity = filterSimilitude },
                    RawSearch = string.IsNullOrWhiteSpace(filter) ? null : new(filter),
                    CreatedAtFrom = createdAtFrom,
                    CreatedAtTo = createdAtTo,
                    IsVisible = isVisible
                },
                joins,
                order,
                paging
            );
            
            if (0 == albums.Total)
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
    public async Task<ActionResult<long>> GetCount(
        [FromQuery] bool filterExact = false,
        [FromQuery] double filterSimilitude = 0.4,
        [FromQuery] string filter = "",
        [FromQuery] DateTime? createdAtFrom = null,
        [FromQuery] DateTime? createdAtTo = null,
        [FromQuery] bool? isVisible = null)
    {
        _logger.LogInformation($"🌍🏳️ API : COUNT albums");
        
        try
        {
            var count = await albumRepository.CountAsync(new AlbumFindQuery
            {
                Search = new() { Exact = filterExact, Similarity = filterSimilitude },
                RawSearch = string.IsNullOrWhiteSpace(filter) ? null : new(filter),
                CreatedAtFrom = createdAtFrom,
                CreatedAtTo = createdAtTo,
                IsVisible = isVisible
            });
            
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
        
        var validationError = ValidateAlbumName(albumDto.Name);
        if (validationError is not null)
        {
            return BadRequest(validationError);
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
        
        var validationError = albumsDto
            .Select(a => ValidateAlbumName(a.Name))
            .FirstOrDefault(err => err is not null);

        if (validationError is not null)
        {
            return BadRequest(validationError);
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