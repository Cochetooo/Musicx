using Microsoft.AspNetCore.Mvc;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.Security;
using Musicx.Application.API.Persistence.Queries;
using Musicx.Application.Shared.Enums;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Security;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Infrastructure.API.Persistence.Specifications.Security;
using Musicx.Presentation.Web.Contexts;

namespace Musicx.Presentation.Web.Controllers.Security;

[ApiController]
[Route("api/roles")]
public sealed class RoleController(IRoleRepository roleRepository,
    ILoggerProvider loggerProvider) : ControllerBase
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(RoleController));
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] long id,
        [FromServices] IUserContext userContext)
    {
        _logger.LogInformation($"🌍🏳️ API : DELETE roles ({id})");
        
        if (false == userContext.Can("role.delete"))
        {
            _logger.LogInformation($"🌍⛔ API : DELETE roles : NOT AUTHORIZED");
            return Unauthorized("Not authorized to delete roles.");
        }

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
    public async Task<IActionResult> DeleteAll([FromRoute] long[] ids,
        [FromServices] IUserContext userContext)
    {
        var stringIds = string.Join(",", ids);
        _logger.LogInformation($"🌍🏳️ API : DELETE ALL roles ({stringIds})");
        
        if (false == userContext.Can("role.delete_all"))
        {
            _logger.LogInformation($"🌍⛔ API : DELETE roles : NOT AUTHORIZED");
            return Unauthorized("Not authorized to delete all roles.");
        }

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
    public async Task<ActionResult<OutRole>> FindById([FromRoute] long id, 
        [FromQuery] RoleJoinSpecification? joins = null)
    {
        _logger.LogInformation($"🌍🏳️ API : FIND BY ID roles ({id})");

        try
        {
            var role = await roleRepository.FindOneByIdAsync(id, joins);

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
    public async Task<ActionResult<OutGenericList<OutRole>>> Find(
        [FromQuery] bool filterExact = false,
        [FromQuery] double filterSimilitude = 0.4,
        [FromQuery] string filter = "",
        [FromQuery] RoleJoinSpecification? joins = null,
        [FromQuery] RoleOrderSpecification? order = null,
        [FromQuery] PagingOptions? paging = null)
    {
        _logger.LogInformation($"🌍🏳️ API : FIND roles");

        try
        {
            var albums = await roleRepository.FindAsync(
                new RoleFindQuery
                {
                    Search = new() { Exact = filterExact, Similarity = filterSimilitude },
                    RawSearch = string.IsNullOrWhiteSpace(filter) ? null : new(filter)
                },
                joins,
                order,
                paging
            );
            
            if (0 == albums.Total)
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
        [FromQuery] RoleJoinSpecification? joins = null,
        [FromQuery] RoleOrderSpecification? order = null)
    {
        var stringIds = string.Join(',', ids);
        _logger.LogInformation($"🌍🏳️ API : FIND BY ID roles ({stringIds})");
        
        if (ids.Length == 0)
        {
            return BadRequest("❌ You must provide at least one ID.");
        }

        try
        {
            var albums = await roleRepository.FindInAsync(ids, joins, order);
            
            _logger.LogInformation($"🌍✅ API : FIND BY ID roles ({stringIds}) - SUCCESS");
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
        _logger.LogInformation($"🌍🏳️ API : COUNT roles");
        
        try
        {
            var count = await roleRepository.CountAsync();
            
            _logger.LogInformation($"🌍✅ API : COUNT roles - SUCCESS");
            return Ok(count);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] InRole roleDto,
        [FromServices] IUserContext userContext)
    {
        _logger.LogInformation($"🌍🏳️ API : SAVE roles");
        
        if (false == userContext.Can("role.save"))
        {
            _logger.LogInformation($"🌍⛔ API : SAVE roles : NOT AUTHORIZED");
            return Unauthorized("Not authorized to save roles.");
        }

        try
        {
            var result = await roleRepository.SaveAsync(roleDto);

            _logger.LogInformation($"🌍✅ API : SAVE roles - SUCCESS");
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ API : SAVE roles - ERROR: {ex.Message}");
            return BadRequest(ex);
        }
    }
    
    [HttpPost("save-all")]
    public async Task<IActionResult> SaveAll([FromBody] IEnumerable<InRole> rolesDto,
        [FromServices] IUserContext userContext)
    {
        _logger.LogInformation($"🌍🏳️ API : SAVE ALL roles");
        
        if (false == userContext.Can("role.save_all"))
        {
            _logger.LogInformation($"🌍⛔ API : SAVE ALL roles : NOT AUTHORIZED");
            return Unauthorized("Not authorized to save roles.");
        }

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