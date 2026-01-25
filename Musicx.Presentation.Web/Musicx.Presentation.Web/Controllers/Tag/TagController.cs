using Microsoft.AspNetCore.Mvc;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.Tag;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Tag;
using Musicx.Contracts.Dto.Responses;
using Musicx.Presentation.Web.Contexts;

namespace Musicx.Presentation.Web.Controllers.Tag;

[ApiController]
[Route("api/tags")]
public sealed class TagController(ITagRepository tagRepository,
    ILoggerProvider loggerProvider) : ControllerBase
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(TagController));
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] long id,
        [FromServices] IUserContext userContext)
    {
        _logger.LogInformation($"🌍🏳️ API : DELETE tags ({id})");
        
        if (false == userContext.Can("tag.delete"))
        {
            _logger.LogInformation($"🌍⛔ API : DELETE tags : NOT AUTHORIZED");
            return Unauthorized("Not authorized to delete tags.");
        }

        try
        {
            await tagRepository.DeleteAsync(id);
                    
            _logger.LogInformation($"🌍✅ API : DELETE tags ({id}) - SUCCESS");
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
        _logger.LogInformation($"🌍🏳️ API : DELETE ALL tags ({stringIds})");
        
        if (false == userContext.Can("tag.delete_all"))
        {
            _logger.LogInformation($"🌍⛔ API : DELETE tags : NOT AUTHORIZED");
            return Unauthorized("Not authorized to delete all tags.");
        }

        try
        {
            await tagRepository.DeleteAllAsync(ids);
                    
            _logger.LogInformation($"🌍✅ API : DELETE ALL tags ({stringIds}) - SUCCESS");
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<OutTag>> FindById([FromRoute] long id)
    {
        _logger.LogInformation($"🌍🏳️ API : FIND BY ID tags ({id})");

        try
        {
            var tag = await tagRepository.FindOneByIdAsync(id);

            if (null == tag)
            {
                _logger.LogInformation($"🌍❔ API : FIND BY ID tags ({id}) - NOT FOUND");
                return NoContent();
            }
            
            _logger.LogInformation($"🌍✅ API : FIND BY ID tags ({id}) - SUCCESS");
            return Ok(tag);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OutTag>>> Find(
        [FromQuery] bool filterExact = false,
        [FromQuery] double filterSimilitude = 0.4,
        [FromQuery] string filter = "")
    {
        _logger.LogInformation($"🌍🏳️ API : FIND tags");

        try
        {
            var albums = await tagRepository.FindAllAsync(
                filterExact, 
                filterSimilitude,
                filter
            );
            
            if (0 == albums.Count)
            {
                _logger.LogInformation($"🌍❔ API : FIND tags - NOT FOUND");
                return NoContent();
            }
            
            _logger.LogInformation($"🌍✅ API : FIND tags - SUCCESS");
            return Ok(albums);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpGet("by-ids")]
    public async Task<ActionResult<IEnumerable<OutTag>>> FindIn(
        [FromQuery] long[] ids)
    {
        var stringIds = string.Join(',', ids);
        _logger.LogInformation($"🌍🏳️ API : FIND BY ID tags ({stringIds})");
        
        if (ids.Length == 0)
        {
            return BadRequest("❌ You must provide at least one ID.");
        }

        try
        {
            var albums = await tagRepository.FindInAsync(ids);
            
            _logger.LogInformation($"🌍✅ API : FIND BY ID tags ({stringIds}) - SUCCESS");
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
        _logger.LogInformation($"🌍🏳️ API : COUNT tags");
        
        try
        {
            var count = await tagRepository.GetCountAsync();
            
            _logger.LogInformation($"🌍✅ API : COUNT tags - SUCCESS");
            return Ok(count);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] InTag tagDto,
        [FromServices] IUserContext userContext)
    {
        _logger.LogInformation($"🌍🏳️ API : SAVE tags");
        
        if (false == userContext.Can("tag.save"))
        {
            _logger.LogInformation($"🌍⛔ API : SAVE tags : NOT AUTHORIZED");
            return Unauthorized("Not authorized to save tags.");
        }

        try
        {
            var result = await tagRepository.SaveAsync(tagDto);

            _logger.LogInformation($"🌍✅ API : SAVE tags - SUCCESS");
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ API : SAVE tags - ERROR: {ex.Message}");
            return BadRequest(ex);
        }
    }
    
    [HttpPost("save-all")]
    public async Task<IActionResult> SaveAll([FromBody] IEnumerable<InTag> tagsDto,
        [FromServices] IUserContext userContext)
    {
        _logger.LogInformation($"🌍🏳️ API : SAVE ALL tags");
        
        if (false == userContext.Can("tag.save_all"))
        {
            _logger.LogInformation($"🌍⛔ API : SAVE ALL tags : NOT AUTHORIZED");
            return Unauthorized("Not authorized to save tags.");
        }

        try
        {
            await tagRepository.SaveAllAsync(tagsDto);

            _logger.LogInformation($"🌍✅ API : SAVE ALL tags - SUCCESS");
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
}