using Microsoft.AspNetCore.Mvc;
using Musicx.Application.Api.Interfaces.Auth;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.User;
using Musicx.Application.Shared.Enums;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Contracts.Dto.Responses;
using Musicx.Infrastructure.API.Persistence.Specifications.User;
using Musicx.Presentation.Web.Contexts;

namespace Musicx.Presentation.Web.Controllers.User;

[ApiController]
[Route("api/users")]
public sealed class UserController(IUserRepository userRepository,
    IAuthService authService,
    ILoggerProvider loggerProvider) : ControllerBase
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(UserController));
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] long id,
        [FromServices] IUserContext userContext)
    {
        _logger.LogInformation($"🌍🏳️ API : DELETE users ({id})");
        
        if (false == userContext.Can("user.delete"))
        {
            _logger.LogInformation($"🌍⛔ API : DELETE users : NOT AUTHORIZED");
            return Unauthorized("Not authorized to delete users.");
        }

        if (false == userContext.Can("moderator.user.delete")
                && id != userContext.CurrentUser?.Id)
        {
            _logger.LogInformation($"🌍⛔ API : DELETE users : NOT AUTHORIZED OTHER USER");
            return Unauthorized("Not authorized to delete another user.");
        }

        try
        {
            await userRepository.DeleteAsync(id);
                    
            _logger.LogInformation($"🌍✅ API : DELETE users ({id}) - SUCCESS");
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
        _logger.LogInformation($"🌍🏳️ API : DELETE ALL users ({stringIds})");
        
        if (false == userContext.Can("user.delete_all"))
        {
            _logger.LogInformation($"🌍⛔ API : DELETE ALL users : NOT AUTHORIZED");
            return Unauthorized("Not authorized to delete all users.");
        }

        try
        {
            await userRepository.DeleteAllAsync(ids);
                    
            _logger.LogInformation($"🌍✅ API : DELETE ALL users ({stringIds}) - SUCCESS");
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<OutUser>> FindById([FromRoute] long id, 
        [FromQuery] UserJoinSpecification? joins = null)
    {
        _logger.LogInformation($"🌍🏳️ API : FIND BY ID users ({id})");

        try
        {
            var user = await userRepository.FindByIdAsync(id, joins);

            if (null == user)
            {
                _logger.LogInformation($"🌍❔ API : FIND BY ID users ({id}) - NOT FOUND");
                return NoContent();
            }
            
            _logger.LogInformation($"🌍✅ API : FIND BY ID users ({id}) - SUCCESS");
            return Ok(user);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OutUser>>> Find(
        [FromQuery] bool filterExact = false,
        [FromQuery] double filterSimilitude = 0.4,
        [FromQuery] string filter = "",
        [FromQuery] UserJoinSpecification? joins = null,
        [FromQuery] UserOrderSpecification? order = null,
        [FromQuery] PagingOptions? paging = null)
    {
        _logger.LogInformation($"🌍🏳️ API : FIND users | filter = {filter}");

        try
        {
            var albums = await userRepository.FindAsync(
                filterExact, 
                filterSimilitude,
                filter, 
                joins,
                order,
                paging
            );
            
            if (0 == albums.Count)
            {
                _logger.LogInformation($"🌍❔ API : FIND users - NOT FOUND");
                return NoContent();
            }
            
            _logger.LogInformation($"🌍✅ API : FIND users - SUCCESS");
            return Ok(albums);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpGet("by-ids")]
    public async Task<ActionResult<IEnumerable<OutUser>>> FindIn(
        [FromQuery] long[] ids,
        [FromQuery] UserJoinSpecification? joins = null,
        [FromQuery] UserOrderSpecification? order = null)
    {
        var stringIds = string.Join(',', ids);
        _logger.LogInformation($"🌍🏳️ API : FIND BY ID users ({stringIds})");
        
        if (ids.Length == 0)
        {
            return BadRequest("❌ You must provide at least one ID.");
        }

        try
        {
            var albums = await userRepository.FindIn(ids, joins, order);
            
            _logger.LogInformation($"🌍✅ API : FIND BY ID users ({stringIds}) - SUCCESS");
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
        _logger.LogInformation($"🌍🏳️ API : COUNT users");
        
        try
        {
            var count = await userRepository.GetCountAsync();
            
            _logger.LogInformation($"🌍✅ API : COUNT users - SUCCESS");
            return Ok(count);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] InUser userDto)
    {
        _logger.LogInformation($"🌍🏳️ API : SAVE users");

        try
        {
            // Convert password -> password hash + salt
            authService.CreatePasswordHash(ref userDto);
            
            // Persist
            await userRepository.SaveAsync(userDto);

            _logger.LogInformation($"🌍✅ API : SAVE users - SUCCESS");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ API : SAVE users - ERROR: {ex.Message}");
            return BadRequest(ex);
        }
    }
    
    [HttpPost("save-all")]
    public async Task<IActionResult> SaveAll([FromBody] IEnumerable<InUser> usersDto)
    {
        _logger.LogInformation($"🌍🏳️ API : SAVE ALL users");

        try
        {
            await userRepository.SaveAllAsync(usersDto);

            _logger.LogInformation($"🌍✅ API : SAVE ALL users - SUCCESS");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ API : SAVE ALL users - ERROR: {ex.Message}");
            return BadRequest(ex);
        }
    }
}