using Microsoft.AspNetCore.Mvc;
using Musicx.Application.Api.Interfaces.Persistence;
using Musicx.Application.Api.Interfaces.Specifications;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Mappers;

namespace Musicx.Presentation.Web.Controllers;

[ApiController]
[Route("api/genres")]
public sealed class GenreController(IGenreRepository genreRepository,
    ILoggerProvider loggerProvider) : ControllerBase
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(GenreController));
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] long id)
    {
        _logger.LogInformation($"🌍🏳️ API : DELETE genres ({id})");

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
    public async Task<IActionResult> DeleteAll([FromRoute] long[] ids)
    {
        var stringIds = string.Join(",", ids);
        _logger.LogInformation($"🌍🏳️ API : DELETE ALL genres ({stringIds})");

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
    public async Task<ActionResult<OutGenre>> FindById([FromRoute] long id, [FromQuery] string query)
    {
        _logger.LogInformation($"🌍🏳️ API : FIND BY ID genres ({id} & includes = {query})");

        try
        {
            var querySpecification = new GenreQuerySpecification
            {
                IncludeChildren = query.Contains("children"),
                IncludeParents = query.Contains("parents")
            };
            
            var genre = await genreRepository.FindByIdAsync(id, querySpecification);

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
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OutGenre>>> Find(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 100, 
        [FromQuery] string filter = "",
        [FromQuery] string query = "")
    {
        _logger.LogInformation($"🌍🏳️ API : FIND genres | includes = {query} & filter = {filter}");
        
        try
        {
            var querySpecification = new GenreQuerySpecification
            {
                IncludeChildren = query.Contains("children"),
                IncludeParents = query.Contains("parents")
            };
            
            var genres = await genreRepository.FindAsync(skip, take, a => 
                string.IsNullOrWhiteSpace(filter) 
                || a.Name.ToLower().StartsWith(filter.ToLower()),
                querySpecification);
            
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
        [FromQuery] string query = "")
    {
        var stringIds = string.Join(',', ids);
        _logger.LogInformation($"🌍🏳️ API : FIND BY ID genres ({stringIds})");
        
        if (ids.Length == 0)
        {
            return BadRequest("❌ You must provide at least one ID.");
        }

        try
        {
            var querySpecification = new GenreQuerySpecification
            {
                IncludeChildren = query.Contains("children"),
                IncludeParents = query.Contains("parents")
            };
            
            var genres = await genreRepository.FindIn(ids, querySpecification);
            
            _logger.LogInformation($"🌍✅ API : FIND BY ID genres ({stringIds}) - SUCCESS");
            return Ok(genres);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpGet("count")]
    public async Task<ActionResult<int>> GetCount()
    {
        _logger.LogInformation($"🌍🏳️ API : COUNT genres");
        
        try
        {
            var count = await genreRepository.GetCountAsync();
            
            _logger.LogInformation($"🌍✅ API : COUNT genres - SUCCESS");
            return Ok(count);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] InGenre genreDto)
    {
        _logger.LogInformation($"🌍🏳️ API : SAVE genres");

        try
        {
            await genreRepository.SaveAsync(genreDto);

            _logger.LogInformation($"🌍✅ API : SAVE genres - SUCCESS");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ API : SAVE genres - ERROR: {ex.Message}");
            return BadRequest(ex);
        }
    }
    
    [HttpPost("save-all")]
    public async Task<IActionResult> SaveAll([FromBody] IEnumerable<InGenre> genresDto)
    {
        _logger.LogInformation($"🌍🏳️ API : SAVE ALL genres");

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