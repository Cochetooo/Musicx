using Microsoft.AspNetCore.Mvc;
using Musicx.Application.Api.Interfaces.Auth;
using Musicx.Application.Api.Interfaces.DataViews;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.User;
using Musicx.Application.Api.Interfaces.Storage;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Web.Interfaces.Models.Auth;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Users;
using Musicx.Contracts.Dto.Responses.User;
using Musicx.Infrastructure.API.Persistence.Mappers;
using Musicx.Infrastructure.API.Persistence.Specifications.User;
using Musicx.Presentation.Web.Contexts;

namespace Musicx.Presentation.Web.Controllers.User;

[ApiController]
[Route("api/users")]
public sealed class UserController(IUserRepository userRepository,
    IUserDataViewBuilder userDataViewBuilder,
    IAuthService authService,
    IAvatarStorage avatarStorage,
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
            var user = await userRepository.FindOneByIdAsync(id, joins);

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
    
    [HttpGet("{id}/data-view")]
    public async Task<ActionResult<OutUserDataView>> FindDataView(
        [FromRoute] long id,
        [FromServices] IUserContext userContext,
        [FromQuery] long? currentUserId = null)
    {
        _logger.LogInformation($"🌍🏳️ API : FIND DATA VIEW users ({id})");

        try
        {
            var effectiveCurrentUserId = currentUserId ?? userContext.CurrentUser?.Id;
            var view = await userDataViewBuilder.BuildAsync(new UserDataViewQuery(id, effectiveCurrentUserId));

            if (view is null)
            {
                _logger.LogInformation($"🌍❔ API : FIND DATA VIEW users ({id}) - NOT FOUND");
                return NoContent();
            }

            _logger.LogInformation($"🌍✅ API : FIND DATA VIEW users ({id}) - SUCCESS");
            return Ok(view);
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
            var albums = await userRepository.FindInAsync(ids, joins, order);
            
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
            var count = await userRepository.CountAsync();
            
            _logger.LogInformation($"🌍✅ API : COUNT users - SUCCESS");
            return Ok(count);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] InUser userDto,
        [FromServices] IUserContext userContext)
    {
        _logger.LogInformation($"🌍🏳️ API : SAVE users");

        try
        {
            if (0 == userDto.Id)
            {
                if (string.IsNullOrWhiteSpace(userDto.Password))
                {
                    return BadRequest("Password is required to create a user.");
                }

                var existingByEmail = await userRepository.FindByEmailAsync(userDto.Email);
                if (existingByEmail is not null)
                {
                    return Conflict("A user with this email already exists.");
                }

                authService.CreatePasswordHash(ref userDto);
            }
            else
            {
                if (userContext.CurrentUser is null)
                {
                    return Unauthorized("You must be authenticated to edit a user.");
                }

                var isSelfEdit = userContext.CurrentUser.Id == userDto.Id;
                var canEditOther = userContext.Can("moderation.user.save");

                if (!isSelfEdit && !canEditOther)
                {
                    return Unauthorized("Not authorized to edit another user.");
                }

                var existingUser = await userRepository.FindOneByIdAsync(userDto.Id);
                if (existingUser is null)
                {
                    return NotFound("User not found.");
                }

                if (!string.Equals(existingUser.Email, userDto.Email, StringComparison.OrdinalIgnoreCase))
                {
                    var userWithEmail = await userRepository.FindByEmailAsync(userDto.Email);
                    if (userWithEmail is not null && userWithEmail.Id != userDto.Id)
                    {
                        return Conflict("A user with this email already exists.");
                    }
                }

                var existingAuth = await userRepository.FindAuthByEmailAsync(existingUser.Email);
                if (existingAuth is null)
                {
                    return BadRequest("Could not resolve user credentials.");
                }

                userDto.PasswordHash = existingAuth.PasswordHash;
                userDto.PasswordSalt = existingAuth.PasswordSalt;
                userDto.Password = null;
            }
            
            // Persist
            var result = await userRepository.SaveAsync(userDto);

            _logger.LogInformation($"🌍✅ API : SAVE users - SUCCESS");
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ API : SAVE users - ERROR: {ex.Message}");
            return BadRequest(ex);
        }
    }
    
    [HttpPost("{id}/change-password")]
    public async Task<IActionResult> ChangePassword([FromRoute] long id,
        [FromBody] InUserPasswordChange request,
        [FromServices] IUserContext userContext)
    {
        _logger.LogInformation($"🌍🏳️ API : CHANGE PASSWORD users ({id})");

        if (string.IsNullOrWhiteSpace(request.NewPassword))
        {
            return BadRequest("New password is required.");
        }

        if (userContext.CurrentUser is null)
        {
            return Unauthorized("You must be authenticated to change a password.");
        }

        var isSelfEdit = userContext.CurrentUser.Id == id;
        var canEditOther = userContext.Can("moderation.user.password.save");

        if (!isSelfEdit && !canEditOther)
        {
            return Unauthorized("Not authorized to change another user's password.");
        }

        try
        {
            var existingUser = await userRepository.FindOneByIdAsync(id);
            if (existingUser is null)
            {
                return NotFound("User not found.");
            }

            var rawUser = existingUser.ToRaw();
            rawUser.Password = request.NewPassword;
            authService.CreatePasswordHash(ref rawUser);

            await userRepository.SaveAsync(rawUser);

            _logger.LogInformation($"🌍✅ API : CHANGE PASSWORD users ({id}) - SUCCESS");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ API : CHANGE PASSWORD users ({id}) - ERROR: {ex.Message}");
            return BadRequest(ex.Message);
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

    [HttpPost("save-avatar")]
    public async Task<ActionResult<string>> SaveAvatar([FromForm] IFormFile? avatar,
        [FromServices] IUserContext userContext)
    {
        _logger.LogInformation($"🌍🏳️ API : SAVE AVATAR");
        
        if (userContext.CurrentUser is null)
        {
            return Unauthorized("User not logged in.");
        }
        
        if (avatar is null || avatar.Length == 0)
        {
            return BadRequest("No file provided");
        }

        try
        {
            var userId = userContext.CurrentUser.Id; // ta méthode existante

            await using var stream = avatar.OpenReadStream();

            var url = await avatarStorage.SaveAsync(
                userId,
                stream,
                avatar.ContentType,
                HttpContext.RequestAborted
            );

            userContext.CurrentUser.PictureUrl = url;
            // Persist new picture url
            await userRepository.SaveAsync(userContext.CurrentUser.ToRaw());

            _logger.LogInformation($"🌍✅ API : SAVE AVATAR - SUCCESS");

            return Ok(url);
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ API : SAVE AVATAR - ERROR: {ex.Message}");
            return BadRequest(ex.Message);
        }
    }
}