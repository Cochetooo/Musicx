using Microsoft.AspNetCore.Mvc;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.Album;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Album;
using Musicx.Presentation.Web.Contexts;

namespace Musicx.Presentation.Web.Controllers.Album;

[ApiController]
[Route("api/album-genres")]
public sealed class AlbumGenreController(
    IAlbumGenreRepository repository,
    ILoggerProvider loggerProvider) : ControllerBase
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(AlbumGenreController));
    
    [HttpPost]
    public async Task<IActionResult> Save([FromBody] InAlbumGenre albumGenreDto, 
        [FromServices] IUserContext userContext)
    {
        _logger.LogInformation($"🌍🏳️ API : SAVE album_genre");
        
        if (false == userContext.Can("user.album.genre.save"))
        {
            _logger.LogInformation($"🌍⛔ API : SAVE album_genre : NOT AUTHORIZED");
            return Unauthorized("Not authorized to save album genres.");
        }
        
        if (false == userContext.Can("moderation.user.album.genre.save")
                && userContext.CurrentUser?.Id != albumGenreDto.TaggerId)
        {
            _logger.LogInformation($"🌍⛔ API : SAVE album_genre : NOT AUTHORIZED OTHER USER");
            return Unauthorized("Not authorized to save album genres of another user.");
        }

        try
        {
            var result = await repository.SaveAsync(albumGenreDto);

            _logger.LogInformation($"🌍✅ API : SAVE album_genre - SUCCESS");
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ API : SAVE album_genre - ERROR: {ex.Message}");
            return BadRequest(ex);
        }
    }
    
    [HttpPost("save-all")]
    public async Task<IActionResult> SaveAll([FromBody] IEnumerable<InAlbumGenre> albumGenresDto,
        [FromServices] IUserContext userContext)
    {
        _logger.LogInformation($"🌍🏳️ API : SAVE ALL album_genre");
        
        if (false == userContext.Can("user.album.genre.save_all"))
        {
            _logger.LogInformation($"🌍⛔ API : SAVE ALL album_genre : NOT AUTHORIZED");
            return Unauthorized("Not authorized to save album genres.");
        }
        
        if (false == userContext.Can("moderation.user.album.genre.save_all"))
        {
            foreach (var albumGenreDto in albumGenresDto)
            {
                if (userContext.CurrentUser?.Id != albumGenreDto.TaggerId)
                {
                    _logger.LogInformation($"🌍⛔ API : SAVE ALL album_genre : NOT AUTHORIZED OTHER USER");
                    return Unauthorized("Not authorized to save album genres of another user.");
                }
            }
        }

        try
        {
            await repository.SaveAllAsync(albumGenresDto);

            _logger.LogInformation($"🌍✅ API : SAVE ALL album_genre - SUCCESS");
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
}