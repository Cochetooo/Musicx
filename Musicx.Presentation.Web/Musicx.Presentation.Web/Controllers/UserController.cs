using Microsoft.AspNetCore.Mvc;
using Musicx.Application.Api.Interfaces.Auth;
using Musicx.Application.Api.Interfaces.Persistence;
using Musicx.Application.Api.Interfaces.Specifications;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Presentation.Web.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UserController(IUserRepository userRepository,
    IAuthService authService,
    ILoggerProvider loggerProvider) : ControllerBase
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(UserController));
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] long id)
    {
        _logger.LogInformation($"🌍🏳️ API : DELETE users ({id})");

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
    public async Task<IActionResult> DeleteAll([FromRoute] long[] ids)
    {
        var stringIds = string.Join(",", ids);
        _logger.LogInformation($"🌍🏳️ API : DELETE ALL users ({stringIds})");

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
    public async Task<ActionResult<OutUser>> FindById([FromRoute] long id, [FromQuery] string query = "")
    {
        _logger.LogInformation($"🌍🏳️ API : FIND BY ID users ({id} & includes = {query})");

        try
        {
            var querySpecification = new UserQuerySpecification
            {
                IncludeRoles = query.Contains("role"),
            };
            
            var user = await userRepository.FindByIdAsync(id, querySpecification);

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
        [FromQuery] int skip = 0,
        [FromQuery] int take = 100, 
        [FromQuery] string filter = "",
        [FromQuery] string query = "")
    {
        _logger.LogInformation($"🌍🏳️ API : FIND users");

        try
        {
            var querySpecification = new UserQuerySpecification
            {
                IncludeRoles = query.Contains("role"),
            };
            
            var albums = await userRepository.FindAsync(skip, take, querySpecification, filter);
            
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
        [FromQuery] string query = "")
    {
        var stringIds = string.Join(',', ids);
        _logger.LogInformation($"🌍🏳️ API : FIND BY ID users ({stringIds})");
        
        if (ids.Length == 0)
        {
            return BadRequest("❌ You must provide at least one ID.");
        }

        try
        {
            var querySpecification = new UserQuerySpecification
            {
                IncludeRoles = query.Contains("role"),
            };
            
            var albums = await userRepository.FindIn(ids, querySpecification);
            
            _logger.LogInformation($"🌍✅ API : FIND BY ID users ({stringIds}) - SUCCESS");
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