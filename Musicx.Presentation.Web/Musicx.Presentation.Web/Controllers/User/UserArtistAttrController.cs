using Microsoft.AspNetCore.Mvc;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.User;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Infrastructure.API.Persistence.Mappers;
using Musicx.Presentation.Web.Contexts;

namespace Musicx.Presentation.Web.Controllers.User;

[ApiController]
[Route("api/user-artist-attrs")]
public sealed class UserArtistAttrController(
    IUserArtistAttrsRepository repository,
    ILoggerProvider loggerProvider) : ControllerBase
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(UserArtistAttrController));

    [HttpDelete("{userId}/{artistId}")]
    public async Task<IActionResult> Delete([FromRoute] long userId, [FromRoute] long artistId,
        [FromServices] IUserContext userContext)
    {
        _logger.LogInformation($"🌍🏳️ API : DELETE user_artist_attrs ({userId},{artistId})");

        if (false == userContext.Can("user.artist.attrs.save"))
        {
            return Unauthorized("Not authorized to update artist attributes.");
        }

        if (false == userContext.Can("moderation.user.artist.attrs.save") && userContext.CurrentUser?.Id != userId)
        {
            return Unauthorized("Not authorized to update artist attributes of another user.");
        }

        try
        {
            var existing = await repository.FindOneAsync(userId, artistId);
            if (existing?.Rating is not null)
            {
                var raw = existing.ToRaw();
                raw.Follow = false;
                await repository.SaveAsync(raw);
            }
            else
            {
                await repository.DeleteAsync(userId, artistId);
            }

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpGet("by-user/{userId}")]
    public async Task<ActionResult<OutGenericList<OutUserArtistAttribute>>> FindByUser([FromRoute] long userId)
    {
        try
        {
            var items = await repository.FindByUserAsync(userId);
            if (items.Count == 0)
            {
                return NoContent();
            }

            return Ok(new OutGenericList<OutUserArtistAttribute>
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

    [HttpGet("by-artist/{artistId}")]
    public async Task<ActionResult<OutGenericList<OutUserArtistAttribute>>> FindByArtist([FromRoute] long artistId)
    {
        try
        {
            var items = await repository.FindByArtistAsync(artistId);
            if (items.Count == 0)
            {
                return NoContent();
            }

            return Ok(new OutGenericList<OutUserArtistAttribute>
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

    [HttpGet("count-by-artist/{artistId}")]
    public async Task<ActionResult<long>> CountByArtist([FromRoute] long artistId)
    {
        try
        {
            return Ok(await repository.CountFollowersByArtistAsync(artistId));
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpGet("{userId}/{artistId}")]
    public async Task<ActionResult<OutUserArtistAttribute?>> FindOne([FromRoute] long userId, [FromRoute] long artistId)
    {
        try
        {
            var item = await repository.FindOneAsync(userId, artistId);
            return item is null ? NoContent() : Ok(item);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] InUserArtistAttribute dto,
        [FromServices] IUserContext userContext)
    {
        _logger.LogInformation("🌍🏳️ API : SAVE user_artist_attrs");

        if (false == userContext.Can("user.artist.attrs.save"))
        {
            return Unauthorized("Not authorized to update artist attributes.");
        }

        if (false == userContext.Can("moderation.user.artist.attrs.save") && userContext.CurrentUser?.Id != dto.UserId)
        {
            return Unauthorized("Not authorized to update artist attributes of another user.");
        }

        try
        {
            var existing = await repository.FindOneAsync(dto.UserId, dto.ArtistId);
            var merged = existing?.ToRaw() ?? new InUserArtistAttribute
            {
                UserId = dto.UserId,
                ArtistId = dto.ArtistId
            };

            merged.Follow = dto.Follow ?? merged.Follow;
            merged.Rating = dto.Rating ?? merged.Rating;

            if ((merged.Follow is null || merged.Follow == false) && merged.Rating is null)
            {
                await repository.DeleteAsync(merged.UserId, merged.ArtistId);
                return Ok(0L);
            }

            var result = await repository.SaveAsync(merged);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
}