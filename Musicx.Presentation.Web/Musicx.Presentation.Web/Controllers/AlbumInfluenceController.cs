using Microsoft.AspNetCore.Mvc;
using Musicx.Application.Api.Interfaces.Persistence;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.Album;
using Musicx.Contracts.Dto.Requests;
using Musicx.Presentation.Web.Contexts;

namespace Musicx.Presentation.Web.Controllers;

[ApiController]
[Route("api/album-influences")]
public sealed class AlbumInfluenceController(
    IAlbumInfluenceRepository repository,
    ILoggerProvider loggerProvider) : ControllerBase
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(AlbumInfluenceController));
    
    [HttpPost]
    public async Task<IActionResult> Save([FromBody] InAlbumInfluence albumInfluenceDto, 
        [FromServices] IUserContext userContext)
    {
        _logger.LogInformation($"🌍🏳️ API : SAVE album_influence");
        
        if (false == userContext.Can("user.album.influence.save"))
        {
            _logger.LogInformation($"🌍⛔ API : SAVE album_influence : NOT AUTHORIZED");
            return Unauthorized("Not authorized to save album influences.");
        }
        
        if (false == userContext.Can("moderation.user.album.influence.save")
                && userContext.CurrentUser?.Id != albumInfluenceDto.TaggerId)
        {
            _logger.LogInformation($"🌍⛔ API : SAVE album_influence : NOT AUTHORIZED OTHER USER");
            return Unauthorized("Not authorized to save album influences of another user.");
        }

        try
        {
            await repository.SaveAsync(albumInfluenceDto);

            _logger.LogInformation($"🌍✅ API : SAVE album_influence - SUCCESS");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ API : SAVE album_influence - ERROR: {ex.Message}");
            return BadRequest(ex);
        }
    }
    
    [HttpPost("save-all")]
    public async Task<IActionResult> SaveAll([FromBody] IEnumerable<InAlbumInfluence> albumInfluencesDto,
        [FromServices] IUserContext userContext)
    {
        _logger.LogInformation($"🌍🏳️ API : SAVE ALL album_influence");
        
        if (false == userContext.Can("user.album.influence.save_all"))
        {
            _logger.LogInformation($"🌍⛔ API : SAVE ALL album_influence : NOT AUTHORIZED");
            return Unauthorized("Not authorized to save album influences.");
        }
        
        if (false == userContext.Can("moderation.user.album.influence.save_all"))
        {
            foreach (var albumInfluenceDto in albumInfluencesDto)
            {
                if (userContext.CurrentUser?.Id != albumInfluenceDto.TaggerId)
                {
                    _logger.LogInformation($"🌍⛔ API : SAVE ALL album_influence : NOT AUTHORIZED OTHER USER");
                    return Unauthorized("Not authorized to save album influences of another user.");
                }
            }
        }

        try
        {
            await repository.SaveAllAsync(albumInfluencesDto);

            _logger.LogInformation($"🌍✅ API : SAVE ALL album_influence - SUCCESS");
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
}