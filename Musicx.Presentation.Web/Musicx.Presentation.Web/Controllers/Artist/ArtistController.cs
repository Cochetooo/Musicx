using Microsoft.AspNetCore.Mvc;
using Musicx.Application.Api.Interfaces.DataViews;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.Artist;
using Musicx.Application.API.Persistence.Queries;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Helpers;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Artist;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Artist;
using Musicx.Contracts.Dto.Responses.Specifics.Artists;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Infrastructure.API.Persistence.Specifications.Artist;
using Musicx.Presentation.Web.Contexts;

namespace Musicx.Presentation.Web.Controllers.Artist;

[ApiController]
[Route("api/artists")]
public sealed class ArtistController(IArtistRepository artistRepository,
    IArtistDataViewBuilder artistDataViewBuilder,
    ILoggerProvider loggerProvider) : ControllerBase
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(ArtistController));
    
    private static string? ValidateArtistName(string? name)
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
        return $"Artist name must use English title case. Expected: '{expected}'.";
    }

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
            var artist = await artistRepository.FindOneByIdAsync(id);

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

    [HttpGet("by-genre/{genreId}")]
    public async Task<ActionResult<OutGenericList<OutArtist>>> FindByGenre(
        [FromRoute] long genreId,
        [FromQuery] PagingOptions? paging = null)
    {
        _logger.LogInformation($"🌍🏳️ API : FIND BY GENRE artists ({genreId})");

        try
        {
            var artists = await artistRepository.FindByGenreIdAsync(
                genreId, 
                paging
            );

            if (0 == artists.Count)
            {
                _logger.LogInformation($"🌍❔ API : FIND BY GENRE artists ({genreId}) - NOT FOUND");
                return NoContent();
            }

            var artistCount = await artistRepository.GetCountByGenreIdAsync(genreId);

            var response = new OutGenericList<OutArtist>
            {
                Items = artists.ToList(),
                Total = artistCount
            };
            
            _logger.LogInformation($"🌍✅ API : FIND BY GENRE artists ({genreId}) - SUCCESS");
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpGet("{id}/data-view")]
    public async Task<ActionResult<OutArtistDataView>> FindDataView(
        [FromRoute] long id,
        [FromServices] IUserContext userContext,
        [FromQuery] long? userId = null)
    {
        _logger.LogInformation($"🌍🏳️ API : FIND DATA VIEW artists ({id})");

        try
        {
            var effectiveUserId = userId ?? userContext.CurrentUser?.Id;
            var view = await artistDataViewBuilder.BuildAsync(new ArtistDataViewQuery(id, effectiveUserId));

            if (view is null)
            {
                _logger.LogInformation($"🌍❔ API : FIND DATA VIEW artists ({id}) - NOT FOUND");
                return NoContent();
            }

            _logger.LogInformation($"🌍✅ API : FIND DATA VIEW artists ({id}) - SUCCESS");
            return Ok(view);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpGet]
    public async Task<ActionResult<OutGenericList<OutArtist>>> Find(
        [FromQuery] bool filterExact = false,
        [FromQuery] double filterSimilitude = 0.4,
        [FromQuery] string filter = "",
        [FromQuery] ArtistJoinSpecification? joins = null,
        [FromQuery] ArtistOrderSpecification? order = null,
        [FromQuery] PagingOptions? paging = null)
    {
        _logger.LogInformation($"🌍🏳️ API : FIND artists | filter = {filter}");

        try
        {
            var artists = await artistRepository.FindAsync(
                new ArtistFindQuery
                {
                    Search = new() { Exact = filterExact, Similarity = filterSimilitude },
                    RawSearch = string.IsNullOrWhiteSpace(filter) ? null : new(filter)
                },
                joins,
                order,
                paging
            );
            
            if (0 == artists.Total)
            {
                _logger.LogInformation($"🌍❔ API : FIND artists - NOT FOUND");
                return NoContent();
            }
            
            _logger.LogInformation($"🌍✅ API : FIND artists - SUCCESS");
            return Ok(artists);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpGet("by-ids")]
    public async Task<ActionResult<IEnumerable<OutArtist>>> FindIn(
        [FromQuery] long[] ids,
        [FromQuery] ArtistJoinSpecification? joins = null,
        [FromQuery] ArtistOrderSpecification? order = null)
    {
        var stringIds = string.Join(',', ids);
        _logger.LogInformation($"🌍🏳️ API : FIND BY ID artists ({stringIds})");
        
        if (ids.Length == 0)
        {
            return BadRequest("❌ You must provide at least one ID.");
        }

        try
        {
            var artists = await artistRepository.FindInAsync(ids, joins, order);
            
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
            var count = await artistRepository.CountAsync();
            
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
        
        var validationError = ValidateArtistName(artistDto.Name);
        if (validationError is not null)
        {
            return BadRequest(validationError);
        }

        try
        {
            var result = await artistRepository.SaveAsync(artistDto);

            _logger.LogInformation($"🌍✅ API : SAVE artists - SUCCESS");
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ API : SAVE artists - ERROR: {ex.Message}");
            return BadRequest($"{ex.Message}");
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
        
        var validationError = artistsDto
            .Select(a => ValidateArtistName(a.Name))
            .FirstOrDefault(err => err is not null);

        if (validationError is not null)
        {
            return BadRequest(validationError);
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