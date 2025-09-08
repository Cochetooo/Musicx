using Microsoft.AspNetCore.Mvc;
using Musicx.Application.Api.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Presentation.Web.Controllers;

[ApiController]
[Route("api/permissions")]
public sealed class PermissionController(IPermissionRepository permissionRepository,
    ILoggerProvider loggerProvider) : ControllerBase
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(PermissionController));
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] long id)
    {
        _logger.LogInformation($"🌍🏳️ API : DELETE permissions ({id})");

        try
        {
            await permissionRepository.DeleteAsync(id);
                    
            _logger.LogInformation($"🌍✅ API : DELETE permissions ({id}) - SUCCESS");
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
        _logger.LogInformation($"🌍🏳️ API : DELETE ALL permissions ({stringIds})");

        try
        {
            await permissionRepository.DeleteAllAsync(ids);
                    
            _logger.LogInformation($"🌍✅ API : DELETE ALL permissions ({stringIds}) - SUCCESS");
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<OutPermission>> FindById([FromRoute] long id, [FromQuery] string query = "")
    {
        _logger.LogInformation($"🌍🏳️ API : FIND BY ID permissions ({id})");

        try
        {
            var permission = await permissionRepository.FindByIdAsync(id);

            if (null == permission)
            {
                _logger.LogInformation($"🌍❔ API : FIND BY ID permissions ({id}) - NOT FOUND");
                return NoContent();
            }
            
            _logger.LogInformation($"🌍✅ API : FIND BY ID permissions ({id}) - SUCCESS");
            return Ok(permission);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OutPermission>>> Find(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 100, 
        [FromQuery] string filter = "",
        [FromQuery] string query = "")
    {
        _logger.LogInformation($"🌍🏳️ API : FIND permissions");

        try
        {
            var albums = await permissionRepository.FindAsync(skip, take, filter: filter);
            
            if (0 == albums.Count)
            {
                _logger.LogInformation($"🌍❔ API : FIND permissions - NOT FOUND");
                return NoContent();
            }
            
            _logger.LogInformation($"🌍✅ API : FIND permissions - SUCCESS");
            return Ok(albums);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpGet("by-ids")]
    public async Task<ActionResult<IEnumerable<OutPermission>>> FindIn(
        [FromQuery] long[] ids,
        [FromQuery] string query = "")
    {
        var stringIds = string.Join(',', ids);
        _logger.LogInformation($"🌍🏳️ API : FIND BY ID permissions ({stringIds})");
        
        if (ids.Length == 0)
        {
            return BadRequest("❌ You must provide at least one ID.");
        }

        try
        {
            var albums = await permissionRepository.FindIn(ids);
            
            _logger.LogInformation($"🌍✅ API : FIND BY ID permissions ({stringIds}) - SUCCESS");
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
        _logger.LogInformation($"🌍🏳️ API : COUNT permissions");
        
        try
        {
            var count = await permissionRepository.GetCountAsync();
            
            _logger.LogInformation($"🌍✅ API : COUNT permissions - SUCCESS");
            return Ok(count);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] InPermission permissionDto)
    {
        _logger.LogInformation($"🌍🏳️ API : SAVE permissions");

        try
        {
            await permissionRepository.SaveAsync(permissionDto);

            _logger.LogInformation($"🌍✅ API : SAVE permissions - SUCCESS");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ API : SAVE permissions - ERROR: {ex.Message}");
            return BadRequest(ex);
        }
    }
    
    [HttpPost("save-all")]
    public async Task<IActionResult> SaveAll([FromBody] IEnumerable<InPermission> permissionsDto)
    {
        _logger.LogInformation($"🌍🏳️ API : SAVE ALL permissions");

        try
        {
            await permissionRepository.SaveAllAsync(permissionsDto);

            _logger.LogInformation($"🌍✅ API : SAVE ALL permissions - SUCCESS");
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
}