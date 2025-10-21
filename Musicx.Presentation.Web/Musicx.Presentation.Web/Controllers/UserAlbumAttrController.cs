using Microsoft.AspNetCore.Mvc;
using Musicx.Application.Api.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Contracts.Dto.Responses.Specifics.Ratings;
using Musicx.Presentation.Web.Contexts;

namespace Musicx.Presentation.Web.Controllers;

[ApiController]
[Route("api/user-album-attrs")]
public sealed class UserAlbumAttrController(
    IUserAlbumAttrsRepository repository,
    ILoggerProvider loggerProvider) : ControllerBase
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(UserAlbumAttrController));

    [HttpDelete("{userId}/{albumId}")]
    public async Task<IActionResult> Delete([FromRoute] long userId, [FromRoute] long albumId,
        [FromServices] IUserContext userContext)
    {
        _logger.LogInformation($"🌍🏳️ API : DELETE user_album_attrs ({userId},{albumId})");
        
        // Permission to delete album attributes
        if (false == userContext.Can("user.album.attrs.delete"))
        {
            _logger.LogInformation($"🌍⛔ API : DELETE user_album_attrs ({userId},{albumId}) : NOT AUTHORIZED");
            return Unauthorized("Not authorized to delete albums attributes.");
        }

        // Permission to delete album attributes of another user
        if (false == userContext.Can("moderation.user.album.attrs.delete")
                && userContext.CurrentUser?.Id != userId)
        {
            _logger.LogInformation($"🌍⛔ API : DELETE user_album_attrs ({userId},{albumId}) : NOT AUTHORIZED OTHER USER");
            return Unauthorized("Not authorized to delete albums attributes of another user.");
        }

        try
        {
            await repository.DeleteAsync(userId, albumId);
                    
            _logger.LogInformation($"🌍✅ API : DELETE user_album_attrs ({userId},{albumId}) - SUCCESS");
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteAll([FromRoute] long userId,
        [FromServices] IUserContext userContext)
    {
        _logger.LogInformation($"🌍🏳️ API : DELETE ALL user_album_attrs ({userId})");
        
        // Permission to delete album attributes
        if (false == userContext.Can("user.album.attrs.delete_all"))
        {
            _logger.LogInformation($"🌍⛔ API : DELETE ALL user_album_attrs ({userId}) : NOT AUTHORIZED");
            return Unauthorized("Not authorized to delete albums attributes.");
        }

        // Permission to delete album attributes of another user
        if (false == userContext.Can("moderation.user.album.attrs.delete_all")
            && userContext.CurrentUser?.Id != userId)
        {
            _logger.LogInformation($"🌍⛔ API : DELETE ALL user_album_attrs ({userId}) : NOT AUTHORIZED OTHER USER");
            return Unauthorized("Not authorized to delete albums attributes of another user.");
        }

        try
        {
            await repository.DeleteAsync(userId);
                    
            _logger.LogInformation($"🌍✅ API : DELETE ALL user_album_attrs ({userId}) - SUCCESS");
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpGet("by-album/{albumId}")]
    public async Task<ActionResult<OutGenericList<OutUserAlbumAttribute>>> FindByAlbumId([FromRoute] long albumId,
        [FromQuery] long skip = 0, [FromQuery] long take = 100)
    {
        _logger.LogInformation($"🌍🏳️ API : FIND BY ALBUM user_album_attrs ({albumId})");

        try
        {
            var userAlbumAttrs = await repository.FindByAlbumIdAsync(albumId, skip, take);
            var totalCount = await repository.CountByAlbumIdAsync(albumId);

            if (0 == userAlbumAttrs.Count)
            {
                _logger.LogInformation($"🌍❔ API : FIND BY ALBUM user_album_attrs ({albumId}) - NOT FOUND");
                return NoContent();
            }
            
            _logger.LogInformation($"🌍✅ API : FIND BY ALBUM user_album_attrs ({albumId}) - SUCCESS");
            return Ok(new OutGenericList<OutUserAlbumAttribute>
            {
                Items = userAlbumAttrs.ToList(),
                Total = totalCount
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpGet("by-user/{userId}")]
    public async Task<ActionResult<OutGenericList<OutUserAlbumAttribute>>> FindByUserId(
        [FromRoute] long userId,
        [FromQuery] long skip = 0, 
        [FromQuery] long take = 100, 
        [FromQuery] bool filterExact = false, 
        [FromQuery] double filterSimilitude = 0.4,
        [FromQuery] string filter = "", 
        [FromQuery] string order = "")
    {
        _logger.LogInformation($"🌍🏳️ API : FIND BY USER user_album_attrs ({userId})");

        try
        {
            var userAlbumAttrs = await repository.FindByUserIdAsync(userId, skip, take,
                filterExact, filterSimilitude, filter, order);
            var totalCount = await repository.CountByUserIdAsync(userId);

            if (0 == userAlbumAttrs.Count)
            {
                _logger.LogInformation($"🌍❔ API : FIND BY USER user_album_attrs ({userId}) - NOT FOUND");
                return NoContent();
            }
            
            _logger.LogInformation($"🌍✅ API : FIND BY USER user_album_attrs ({userId}) - SUCCESS");
            return Ok(new OutGenericList<OutUserAlbumAttribute>
            {
                Items = userAlbumAttrs.ToList(),
                Total = totalCount
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpGet("by-user/{userId}/{albumId}")]
    public async Task<ActionResult<OutUserAlbumAttribute>> FindOneAlbumFromUser([FromRoute] long userId,
        [FromRoute] long albumId)
    {
        _logger.LogInformation($"🌍🏳️ API : FIND ALBUM FROM USER user_album_attrs ({userId},{albumId})");

        try
        {
            var userAlbumAttr = await repository.FindOneAlbumFromUserAsync(userId, albumId);

            if (userAlbumAttr is null)
            {
                _logger.LogInformation($"🌍❔ API : FIND ALBUM FROM USER user_album_attrs ({userId},{albumId}) - NOT FOUND");
                return NoContent();
            }
            
            _logger.LogInformation($"🌍✅ API : FIND ALBUM FROM USER user_album_attrs ({userId},{albumId}) - SUCCESS");
            return Ok(userAlbumAttr);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpGet("count-by-album/{albumId}")]
    public async Task<IActionResult> GetCountByAlbumId([FromRoute] long albumId)
    {
        _logger.LogInformation($"🌍🏳️ API : COUNT user_album_attrs BY album {albumId}");

        try
        {
            var count = await repository.CountByAlbumIdAsync(albumId);

            _logger.LogInformation($"🌍✅ API : COUNT user_album_attrs BY album {albumId} - SUCCESS");
            return Ok(count);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpGet("count-by-user/{userId}")]
    public async Task<IActionResult> GetCountByUserId([FromRoute] long userId)
    {
        _logger.LogInformation($"🌍🏳️ API : COUNT user_album_attrs BY user {userId}");

        try
        {
            var count = await repository.CountByUserIdAsync(userId);

            _logger.LogInformation($"🌍✅ API : COUNT user_album_attrs BY user {userId} - SUCCESS");
            return Ok(count);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpGet("ratings-distrib/{userId}")]
    public async Task<ActionResult<OutUserRatingStats>> GetRatingsDistribByUserId([FromRoute] long userId)
    {
        _logger.LogInformation($"🌍🏳️ API : RATING DISTRIB user_album_attrs BY user {userId}");

        try
        {
            var ratingDistrib = await repository.GetUserRatingStatsAsync(userId);
            
            _logger.LogInformation($"🌍✅ API : RATING DISTRIB user_album_attrs BY user {userId} - SUCCESS");
            return Ok(ratingDistrib);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> Save([FromBody] InUserAlbumAttribute userAlbumAttrDto, 
        [FromServices] IUserContext userContext)
    {
        _logger.LogInformation($"🌍🏳️ API : SAVE user_album_attrs");
        
        if (false == userContext.Can("user.album.attrs.save"))
        {
            _logger.LogInformation($"🌍⛔ API : SAVE user_album_attrs : NOT AUTHORIZED");
            return Unauthorized("Not authorized to save albums.");
        }
        
        if (false == userContext.Can("moderation.user.album.attrs.save")
                && userContext.CurrentUser?.Id != userAlbumAttrDto.UserId)
        {
            _logger.LogInformation($"🌍⛔ API : SAVE user_album_attrs : NOT AUTHORIZED OTHER USER");
            return Unauthorized("Not authorized to save albums of another user.");
        }

        try
        {
            await repository.SaveAsync(userAlbumAttrDto);

            _logger.LogInformation($"🌍✅ API : SAVE user_album_attrs - SUCCESS");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ API : SAVE user_album_attrs - ERROR: {ex.Message}");
            return BadRequest(ex);
        }
    }
}