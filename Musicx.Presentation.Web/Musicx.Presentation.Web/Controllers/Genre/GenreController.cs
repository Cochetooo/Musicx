using Microsoft.AspNetCore.Mvc;
using Musicx.Application.Api.Interfaces.DataViews;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.Genre;
using Musicx.Application.Shared.Enums;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Genre;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Genre;
using Musicx.Contracts.Dto.Responses.Specifics.Genres;
using Musicx.Infrastructure.API.Persistence.Specifications.Genre;
using Musicx.Presentation.Web.Contexts;

namespace Musicx.Presentation.Web.Controllers.Genre;

[ApiController]
[Route("api/genres")]
public sealed class GenreController(IGenreRepository genreRepository,
    IGenreDataViewBuilder genreDataViewBuilder,
    ILoggerProvider loggerProvider) : ControllerBase
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(GenreController));
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] long id,
        [FromServices] IUserContext userContext)
    {
        _logger.LogInformation($"🌍🏳️ API : DELETE genres ({id})");
        
        if (false == userContext.Can("genre.delete"))
        {
            _logger.LogInformation($"🌍⛔ API : DELETE genres : NOT AUTHORIZED");
            return Unauthorized("Not authorized to delete genres.");
        }

        try
        {
            await genreRepository.DeleteAsync(id);
                    
            _logger.LogInformation($"🌍✅ API : DELETE genres ({id}) - SUCCESS");
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
        _logger.LogInformation($"🌍🏳️ API : DELETE ALL genres ({stringIds})");
        
        if (false == userContext.Can("genre.delete_all"))
        {
            _logger.LogInformation($"🌍⛔ API : DELETE ALL genres : NOT AUTHORIZED");
            return Unauthorized("Not authorized to delete all genres.");
        }

        try
        {
            await genreRepository.DeleteAllAsync(ids);
                    
            _logger.LogInformation($"🌍✅ API : DELETE ALL genres ({stringIds}) - SUCCESS");
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OutGenre>> FindById([FromRoute] long id, 
        [FromQuery] GenreJoinSpecification? joins = null)
    {
        _logger.LogInformation($"🌍🏳️ API : FIND BY ID genres ({id})");

        try
        {
            var genre = await genreRepository.FindOneByIdAsync(id, joins);

            if (null == genre)
            {
                _logger.LogInformation($"🌍❔ API : FIND BY ID genres ({id}) - NOT FOUND");
                return NoContent();
            }
            
            _logger.LogInformation($"🌍✅ API : FIND BY ID genres ({id}) - SUCCESS");
            return Ok(genre);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpGet("{id}/data-view")]
    public async Task<ActionResult<OutGenreDataView>> FindDataView(
        [FromRoute] long id,
        [FromServices] IUserContext userContext,
        [FromQuery] long? userId = null)
    {
        _logger.LogInformation($"🌍🏳️ API : FIND DATA VIEW genres ({id})");

        try
        {
            var effectiveUserId = userId ?? userContext.CurrentUser?.Id;
            var view = await genreDataViewBuilder.BuildAsync(new GenreDataViewQuery(id, effectiveUserId));

            if (view is null)
            {
                _logger.LogInformation($"🌍❔ API : FIND DATA VIEW genres ({id}) - NOT FOUND");
                return NoContent();
            }

            _logger.LogInformation($"🌍✅ API : FIND DATA VIEW genres ({id}) - SUCCESS");
            return Ok(view);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OutGenre>>> Find(
        [FromQuery] bool filterExact = false,
        [FromQuery] double filterSimilitude = 0.4,
        [FromQuery] string filter = "",
        [FromQuery] GenreJoinSpecification? joins = null,
        [FromQuery] GenreOrderSpecification? order = null,
        [FromQuery] PagingOptions? paging = null)
    {
        _logger.LogInformation($"🌍🏳️ API : FIND genres | filter = {filter}");
        
        try
        {
            var genres = await genreRepository.FindAsync(
                filterExact, 
                filterSimilitude, 
                filter,
                joins,
                order,
                paging
            );
            
            if (0 == genres.Count)
            {
                _logger.LogInformation($"🌍❔ API : FIND genres - NOT FOUND");
                return NoContent();
            }
            
            _logger.LogInformation($"🌍✅ API : FIND genres - SUCCESS");
            return Ok(genres);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpGet("by-ids")]
    public async Task<ActionResult<IEnumerable<OutGenre>>> FindIn(
        [FromQuery] long[] ids,
        [FromQuery] GenreJoinSpecification? joins = null,
        [FromQuery] GenreOrderSpecification? order = null)
    {
        var stringIds = string.Join(',', ids);
        _logger.LogInformation($"🌍🏳️ API : FIND BY ID genres ({stringIds})");
        
        if (ids.Length == 0)
        {
            return BadRequest("❌ You must provide at least one ID.");
        }

        try
        {
            var genres = await genreRepository.FindInAsync(ids, joins, order);
            
            _logger.LogInformation($"🌍✅ API : FIND BY ID genres ({stringIds}) - SUCCESS");
            return Ok(genres);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpGet("count")]
    public async Task<ActionResult<long>> GetCount()
    {
        _logger.LogInformation($"🌍🏳️ API : COUNT genres");
        
        try
        {
            var count = await genreRepository.CountAsync();
            
            _logger.LogInformation($"🌍✅ API : COUNT genres - SUCCESS");
            return Ok(count);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] InGenre genreDto,
        [FromServices] IUserContext userContext)
    {
        _logger.LogInformation($"🌍🏳️ API : SAVE genres");
        
        if (false == userContext.Can("genre.save"))
        {
            _logger.LogInformation($"🌍⛔ API : SAVE genres : NOT AUTHORIZED");
            return Unauthorized("Not authorized to save genres.");
        }

        try
        {
            var result = await genreRepository.SaveAsync(genreDto);

            _logger.LogInformation($"🌍✅ API : SAVE genres - SUCCESS");
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ API : SAVE genres - ERROR: {ex.Message}");
            return BadRequest(ex);
        }
    }
    
    [HttpPost("save-all")]
    public async Task<IActionResult> SaveAll([FromBody] IEnumerable<InGenre> genresDto,
        [FromServices] IUserContext userContext)
    {
        _logger.LogInformation($"🌍🏳️ API : SAVE ALL genres");
        
        if (false == userContext.Can("genre.save_all"))
        {
            _logger.LogInformation($"🌍⛔ API : SAVE ALL genres : NOT AUTHORIZED");
            return Unauthorized("Not authorized to save genres.");
        }

        try
        {
            await genreRepository.SaveAllAsync(genresDto);

            _logger.LogInformation($"🌍✅ API : SAVE ALL genres - SUCCESS");
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
}