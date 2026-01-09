using Microsoft.AspNetCore.Mvc;
using Musicx.Application.Api.Interfaces.Persistence;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.Security;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Presentation.Web.Contexts;

namespace Musicx.Presentation.Web.Controllers;

[ApiController]
[Route("api/permissions")]
public sealed class PermissionController(IPermissionRepository permissionRepository,
    ILoggerProvider loggerProvider) : ControllerBase
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(PermissionController));
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] long id,
        [FromServices] IUserContext userContext)
    {
        _logger.LogInformation($"🌍🏳️ API : DELETE permissions ({id})");
        
        if (false == userContext.Can("permission.delete"))
        {
            _logger.LogInformation($"🌍⛔ API : DELETE permissions : NOT AUTHORIZED");
            return Unauthorized("Not authorized to delete permissions.");
        }

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
    public async Task<IActionResult> DeleteAll([FromRoute] long[] ids,
        [FromServices] IUserContext userContext)
    {
        var stringIds = string.Join(",", ids);
        _logger.LogInformation($"🌍🏳️ API : DELETE ALL permissions ({stringIds})");
        
        if (false == userContext.Can("permission.delete_all"))
        {
            _logger.LogInformation($"🌍⛔ API : DELETE ALL permissions : NOT AUTHORIZED");
            return Unauthorized("Not authorized to delete all permissions.");
        }
        
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
        [FromQuery] long skip = 0,
        [FromQuery] long take = 100, 
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
    public async Task<ActionResult<long>> GetCount()
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
    public async Task<IActionResult> Save([FromBody] InPermission permissionDto,
        [FromServices] IUserContext userContext)
    {
        _logger.LogInformation($"🌍🏳️ API : SAVE permissions");
        
        if (false == userContext.Can("permission.save"))
        {
            _logger.LogInformation($"🌍⛔ API : SAVE permissions : NOT AUTHORIZED");
            return Unauthorized("Not authorized to save permissions.");
        }

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
    public async Task<IActionResult> SaveAll([FromBody] IEnumerable<InPermission> permissionsDto,
        [FromServices] IUserContext userContext)
    {
        _logger.LogInformation($"🌍🏳️ API : SAVE ALL permissions");
        
        if (false == userContext.Can("permission.save_all"))
        {
            _logger.LogInformation($"🌍⛔ API : SAVE ALL permissions : NOT AUTHORIZED");
            return Unauthorized("Not authorized to save permissions.");
        }

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