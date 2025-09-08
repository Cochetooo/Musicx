using Microsoft.AspNetCore.Mvc;
using Musicx.Application.Api.Interfaces.Persistence;
using Musicx.Application.Api.Interfaces.Specifications;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Presentation.Web.Controllers;

[ApiController]
[Route("api/roles")]
public sealed class RoleController(IRoleRepository roleRepository,
    ILoggerProvider loggerProvider) : ControllerBase
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(RoleController));
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] long id)
    {
        _logger.LogInformation($"🌍🏳️ API : DELETE roles ({id})");

        try
        {
            await roleRepository.DeleteAsync(id);
                    
            _logger.LogInformation($"🌍✅ API : DELETE roles ({id}) - SUCCESS");
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
        _logger.LogInformation($"🌍🏳️ API : DELETE ALL roles ({stringIds})");

        try
        {
            await roleRepository.DeleteAllAsync(ids);
                    
            _logger.LogInformation($"🌍✅ API : DELETE ALL roles ({stringIds}) - SUCCESS");
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<OutRole>> FindById([FromRoute] long id, [FromQuery] string query = "")
    {
        _logger.LogInformation($"🌍🏳️ API : FIND BY ID roles ({id} & includes = {query})");

        try
        {
            var querySpecification = new RoleQuerySpecification
            {
                IncludePermissions = query.Contains("permission"),
            };
            
            var role = await roleRepository.FindByIdAsync(id, querySpecification);

            if (null == role)
            {
                _logger.LogInformation($"🌍❔ API : FIND BY ID roles ({id}) - NOT FOUND");
                return NoContent();
            }
            
            _logger.LogInformation($"🌍✅ API : FIND BY ID roles ({id}) - SUCCESS");
            return Ok(role);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OutRole>>> Find(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 100, 
        [FromQuery] string filter = "",
        [FromQuery] string query = "")
    {
        _logger.LogInformation($"🌍🏳️ API : FIND roles");

        try
        {
            var querySpecification = new RoleQuerySpecification
            {
                IncludePermissions = query.Contains("permission"),
            };
            
            var albums = await roleRepository.FindAsync(skip, take, querySpecification, filter);
            
            if (0 == albums.Count)
            {
                _logger.LogInformation($"🌍❔ API : FIND roles - NOT FOUND");
                return NoContent();
            }
            
            _logger.LogInformation($"🌍✅ API : FIND roles - SUCCESS");
            return Ok(albums);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpGet("by-ids")]
    public async Task<ActionResult<IEnumerable<OutRole>>> FindIn(
        [FromQuery] long[] ids,
        [FromQuery] string query = "")
    {
        var stringIds = string.Join(',', ids);
        _logger.LogInformation($"🌍🏳️ API : FIND BY ID roles ({stringIds})");
        
        if (ids.Length == 0)
        {
            return BadRequest("❌ You must provide at least one ID.");
        }

        try
        {
            var querySpecification = new RoleQuerySpecification
            {
                IncludePermissions = query.Contains("permission"),
            };
            
            var albums = await roleRepository.FindIn(ids, querySpecification);
            
            _logger.LogInformation($"🌍✅ API : FIND BY ID roles ({stringIds}) - SUCCESS");
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
        _logger.LogInformation($"🌍🏳️ API : COUNT roles");
        
        try
        {
            var count = await roleRepository.GetCountAsync();
            
            _logger.LogInformation($"🌍✅ API : COUNT roles - SUCCESS");
            return Ok(count);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] InRole roleDto)
    {
        _logger.LogInformation($"🌍🏳️ API : SAVE roles");

        try
        {
            await roleRepository.SaveAsync(roleDto);

            _logger.LogInformation($"🌍✅ API : SAVE roles - SUCCESS");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ API : SAVE roles - ERROR: {ex.Message}");
            return BadRequest(ex);
        }
    }
    
    [HttpPost("save-all")]
    public async Task<IActionResult> SaveAll([FromBody] IEnumerable<InRole> rolesDto)
    {
        _logger.LogInformation($"🌍🏳️ API : SAVE ALL roles");

        try
        {
            await roleRepository.SaveAllAsync(rolesDto);

            _logger.LogInformation($"🌍✅ API : SAVE ALL roles - SUCCESS");
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
}