using Microsoft.AspNetCore.Mvc;
using Musicx.Application.Api.Interfaces.PatchNotes;

namespace Musicx.Presentation.Web.Controllers.PatchNotes;

[ApiController]
[Route("api/patch-notes")]
public sealed class PatchNotesController(
    IPatchNoteService patchNoteService,
    ILoggerProvider loggerProvider) : ControllerBase
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(PatchNotesController));
    
    [HttpGet]
    public IActionResult GetVersions()
    {
        _logger.LogInformation("🌍🏳️ API : LIST patch-notes");
        _logger.LogInformation("🌍✅ API : LIST patch-notes - SUCCESS");
        return Ok(patchNoteService.GetVersions());
    }
}