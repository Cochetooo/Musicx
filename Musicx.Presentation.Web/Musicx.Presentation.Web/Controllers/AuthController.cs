using Microsoft.AspNetCore.Mvc;
using Musicx.Application.Api.Interfaces.Auth;
using Musicx.Contracts.Dto.Requests.Specifics;

namespace Musicx.Presentation.Web.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IAuthService authService,
    ILoggerProvider loggerProvider) : ControllerBase
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(AuthController));

    [HttpPost("signin")]
    public async Task<ActionResult<string>> SignIn([FromBody] SignInRequest request)
    {
        _logger.LogInformation($"🌍🏳️ API : SIGNIN user {request.Email}");
        
        try
        {
            var response = await authService.SignInAsync(request.Email, request.Password);

            _logger.LogInformation($"🌍✅ API : SIGNIN user {request.Email} - SUCCESS");
            
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError($"❌ API : SIGNIN user {request.Email} UNAUTHORIZED : {ex.Message}");
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ API : SIGNIN user {request.Email} ERROR : {ex.Message}");
            return BadRequest(ex);
        }
    }
}