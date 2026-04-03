using Microsoft.AspNetCore.Mvc;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.User;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Contracts.Dto.Responses.User;
using Musicx.Presentation.Web.Contexts;

namespace Musicx.Presentation.Web.Controllers.User;

[ApiController]
[Route("api/user-song-attrs")]
public sealed class UserSongAttrController(
    IUserSongAttrsRepository repository,
    ILoggerProvider loggerProvider) : ControllerBase
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(UserSongAttrController));

    [HttpDelete("{userId}/{songId}")]
    public async Task<IActionResult> Delete([FromRoute] long userId, [FromRoute] long songId,
        [FromServices] IUserContext userContext)
    {
        _logger.LogInformation("🌍🏳️ API : DELETE user_song_attrs ({UserId},{SongId})", userId, songId);

        if (false == userContext.Can("album.rate") && userContext.CurrentUser?.Id != userId)
        {
            return Unauthorized("Not authorized to delete song attributes.");
        }

        try
        {
            await repository.DeleteAsync(userId, songId);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpGet("by-user/{userId}/{songId}")]
    public async Task<ActionResult<OutUserSongAttribute?>> FindOneBySongUser([FromRoute] long userId,
        [FromRoute] long songId)
    {
        try
        {
            var item = await repository.FindOneBySongUserAsync(userId, songId);
            return item is null ? NoContent() : Ok(item);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpGet("by-user/{userId}/album/{albumId}")]
    public async Task<ActionResult<OutGenericList<OutUserSongAttribute>>> FindByAlbumUser([FromRoute] long userId,
        [FromRoute] long albumId)
    {
        try
        {
            var items = await repository.FindByAlbumUserAsync(userId, albumId);
            if (items.Count == 0)
            {
                return NoContent();
            }

            return Ok(new OutGenericList<OutUserSongAttribute>
            {
                Items = items.ToList(),
                Total = items.Count
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] InUserSongAttribute dto,
        [FromServices] IUserContext userContext)
    {
        _logger.LogInformation("🌍🏳️ API : SAVE user_song_attrs");

        if (false == userContext.Can("album.rate") && userContext.CurrentUser?.Id != dto.UserId)
        {
            return Unauthorized("Not authorized to save song attributes.");
        }

        try
        {
            if (dto.Rating == null
                && dto.ProductionRating == null
                && dto.LyricsRating == null
                && dto.InstrumentationRating == null
                && dto.VocalsRating == null
                && dto.AtmosphereRating == null
                && dto.OriginalityRating == null)
            {
                await repository.DeleteAsync(dto.UserId, dto.SongId);
                return Ok(0L);
            }

            var result = await repository.SaveAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
}