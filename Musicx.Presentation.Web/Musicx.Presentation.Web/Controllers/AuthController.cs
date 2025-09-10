using Microsoft.AspNetCore.Mvc;
using Musicx.Application.Api.Interfaces.Auth;
using Musicx.Contracts.Dto.Requests.Specifics;
using Musicx.Presentation.Web.Contexts;

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
            
            Response.Cookies.Append("AuthToken", response, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });

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

    [HttpPost("signout")]
    public IActionResult LogOut()
    {
        _logger.LogInformation($"🌍🏳️ API : LOGOUT user");
        
        Response.Cookies.Append("AuthToken", "", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddDays(-1)
        });
        
        _logger.LogInformation($"🌍✅ API : LOGOUT user - SUCCESS");

        return Ok();
    }

    [HttpGet("me")]
    public IActionResult Me([FromServices] IUserContext userContext)
    {
        _logger.LogInformation($"🌍🏳️ API : ME user {userContext.CurrentUser?.Name ?? "[null]"}");
        
        if (userContext.CurrentUser is null)
        {
            _logger.LogInformation($"🌍⛔ API : ME user {userContext.CurrentUser?.Name ?? "[null]"} : Not Authorized");
            return Unauthorized();
        }
        
        _logger.LogInformation($"🌍✅ API : ME user {userContext.CurrentUser?.Name ?? "[null]"} - SUCCESS");

        return Ok(new
        {
            userContext.CurrentUser,
            userContext.Roles,
            userContext.Permissions
        });
    }
}