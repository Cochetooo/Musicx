using Microsoft.AspNetCore.Mvc;
using Musicx.Application.Api.Interfaces.Persistence;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.Genre;
using Musicx.Contracts.Dto.Requests;
using Musicx.Presentation.Web.Contexts;

namespace Musicx.Presentation.Web.Controllers;

[ApiController]
[Route("api/genre-relations")]
public sealed class GenreRelationController(
    IGenreRelationRepository repository,
    ILoggerProvider loggerProvider) : ControllerBase
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(GenreRelationController));
    
    [HttpPost]
    public async Task<IActionResult> Save([FromBody] InGenreRelation genreRelationDto, 
        [FromServices] IUserContext userContext)
    {
        _logger.LogInformation($"🌍🏳️ API : SAVE genre_relation");
        
        if (false == userContext.Can("genre.save"))
        {
            _logger.LogInformation($"🌍⛔ API : SAVE genre_relation : NOT AUTHORIZED");
            return Unauthorized("Not authorized to save genre relations.");
        }

        try
        {
            await repository.SaveAsync(genreRelationDto);

            _logger.LogInformation($"🌍✅ API : SAVE genre_relation - SUCCESS");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ API : SAVE genre_relation - ERROR: {ex.Message}");
            return BadRequest(ex);
        }
    }
    
    [HttpPost("save-all")]
    public async Task<IActionResult> SaveAll([FromBody] IEnumerable<InGenreRelation> genreRelationsDto,
        [FromServices] IUserContext userContext)
    {
        _logger.LogInformation($"🌍🏳️ API : SAVE ALL genre_relation");
        
        if (false == userContext.Can("genre.save_all"))
        {
            _logger.LogInformation($"🌍⛔ API : SAVE ALL genre_relation : NOT AUTHORIZED");
            return Unauthorized("Not authorized to save genre relations.");
        }

        try
        {
            await repository.SaveAllAsync(genreRelationsDto);

            _logger.LogInformation($"🌍✅ API : SAVE ALL genre_relation - SUCCESS");
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
}