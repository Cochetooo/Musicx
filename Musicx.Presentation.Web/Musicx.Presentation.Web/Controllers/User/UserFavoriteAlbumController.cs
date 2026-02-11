using Microsoft.AspNetCore.Mvc;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.User;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Contracts.Dto.Responses.User;
using Musicx.Presentation.Web.Contexts;

namespace Musicx.Presentation.Web.Controllers.User;

[ApiController]
[Route("api/user-fav-album")]
public sealed class UserFavoriteAlbumController(
    IUserFavoriteAlbumRepository repository,
    ILoggerProvider loggerProvider) : ControllerBase
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(UserFavoriteAlbumController));

    [HttpDelete("{userId}/{albumId}")]
    public async Task<IActionResult> DeleteOne([FromRoute] long userId, [FromRoute] long albumId,
        [FromServices] IUserContext userContext)
    {
        _logger.LogInformation($"🌍🏳️ API : DELETE user_fav_album ({userId},{albumId})");
        
        // Permission to delete album attributes
        if (false == userContext.Can("user.album.fav.delete"))
        {
            _logger.LogInformation($"🌍⛔ API : DELETE user_fav_album ({userId},{albumId}) : NOT AUTHORIZED");
            return Unauthorized("Not authorized to delete albums attributes.");
        }

        // Permission to delete album attributes of another user
        if (false == userContext.Can("moderation.user.album.fav.delete")
                && userContext.CurrentUser?.Id != userId)
        {
            _logger.LogInformation($"🌍⛔ API : DELETE user_fav_album ({userId},{albumId}) : NOT AUTHORIZED OTHER USER");
            return Unauthorized("Not authorized to delete albums attributes of another user.");
        }

        try
        {
            await repository.DeleteOneAsync(userId, albumId);
                    
            _logger.LogInformation($"🌍✅ API : DELETE user_fav_album ({userId},{albumId}) - SUCCESS");
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpDelete("{userId}")]
    public async Task<IActionResult> Delete([FromRoute] long userId,
        [FromServices] IUserContext userContext)
    {
        _logger.LogInformation($"🌍🏳️ API : DELETE ALL user_fav_album ({userId})");
        
        // Permission to delete album attributes
        if (false == userContext.Can("user.album.fav.delete_all"))
        {
            _logger.LogInformation($"🌍⛔ API : DELETE ALL user_fav_album ({userId}) : NOT AUTHORIZED");
            return Unauthorized("Not authorized to delete albums attributes.");
        }

        // Permission to delete album attributes of another user
        if (false == userContext.Can("moderation.user.album.fav.delete_all")
            && userContext.CurrentUser?.Id != userId)
        {
            _logger.LogInformation($"🌍⛔ API : DELETE ALL user_fav_album ({userId}) : NOT AUTHORIZED OTHER USER");
            return Unauthorized("Not authorized to delete albums attributes of another user.");
        }

        try
        {
            await repository.DeleteAsync(userId);
                    
            _logger.LogInformation($"🌍✅ API : DELETE ALL user_fav_album ({userId}) - SUCCESS");
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpGet("by-user/{userId}")]
    public async Task<ActionResult<OutGenericList<OutUserFavoriteAlbum>>> FindByUserId(
        [FromRoute] long userId)
    {
        _logger.LogInformation($"🌍🏳️ API : FIND BY USER user_fav_album ({userId})");

        try
        {
            var userFavouriteAlbums = await repository.FindByUserAsync(
                userId
            );

            if (0 == userFavouriteAlbums.Count)
            {
                _logger.LogInformation($"🌍❔ API : FIND BY USER user_fav_album ({userId}) - NOT FOUND");
                return NoContent();
            }
            
            _logger.LogInformation($"🌍✅ API : FIND BY USER user_fav_album ({userId}) - SUCCESS");
            return Ok(new OutGenericList<OutUserFavoriteAlbum>
            {
                Items = userFavouriteAlbums.ToList(),
                Total = userFavouriteAlbums.Count
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> Save([FromBody] InUserFavoriteAlbum userFavAlbumDto, 
        [FromServices] IUserContext userContext)
    {
        _logger.LogInformation($"🌍🏳️ API : SAVE user_fav_album");
        
        if (false == userContext.Can("user.album.fav.save"))
        {
            _logger.LogInformation($"🌍⛔ API : SAVE user_fav_album : NOT AUTHORIZED");
            return Unauthorized("Not authorized to save album genres.");
        }
        
        if (false == userContext.Can("moderation.user.album.fav.save")
                && userContext.CurrentUser?.Id != userFavAlbumDto.UserId)
        {
            _logger.LogInformation($"🌍⛔ API : SAVE user_fav_album : NOT AUTHORIZED OTHER USER");
            return Unauthorized("Not authorized to save album genres of another user.");
        }

        try
        {
            var result = await repository.SaveAsync(userFavAlbumDto);

            _logger.LogInformation($"🌍✅ API : SAVE user_fav_album - SUCCESS");
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ API : SAVE user_fav_album - ERROR: {ex.Message}");
            return BadRequest(ex);
        }
    }
    
    [HttpPost("save-all")]
    public async Task<IActionResult> SaveAll([FromBody] IEnumerable<InUserFavoriteAlbum> userFavAlbumsDto,
        [FromServices] IUserContext userContext)
    {
        _logger.LogInformation($"🌍🏳️ API : SAVE ALL user_fav_album");
        
        if (false == userContext.Can("user.album.fav.save_all"))
        {
            _logger.LogInformation($"🌍⛔ API : SAVE ALL user_fav_album : NOT AUTHORIZED");
            return Unauthorized("Not authorized to save album genres.");
        }
        
        if (false == userContext.Can("moderation.user.album.fav.save_all"))
        {
            foreach (var albumGenreDto in userFavAlbumsDto)
            {
                if (userContext.CurrentUser?.Id != albumGenreDto.UserId)
                {
                    _logger.LogInformation($"🌍⛔ API : SAVE ALL user_fav_album : NOT AUTHORIZED OTHER USER");
                    return Unauthorized("Not authorized to save album genres of another user.");
                }
            }
        }

        try
        {
            await repository.SaveAllAsync(userFavAlbumsDto);

            _logger.LogInformation($"🌍✅ API : SAVE ALL user_fav_album - SUCCESS");
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
}