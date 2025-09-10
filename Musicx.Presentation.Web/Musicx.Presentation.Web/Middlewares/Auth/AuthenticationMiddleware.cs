using Musicx.Application.Api.Interfaces.Auth;

namespace Musicx.Presentation.Web.Middlewares.Auth;

public sealed class AuthenticationMiddleware(
    RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ITokenValidator tokenValidator)
    {
        string? token = null;
        
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            token = authHeader.Substring("Bearer ".Length).Trim();
        }
        else
        {
            if (context.Request.Cookies.TryGetValue("AuthToken", out var cookieToken))
            {
                token = cookieToken;
            }
        }

        if (!string.IsNullOrEmpty(token))
        {
            var principal = tokenValidator.ValidateToken(token);
            if (principal != null)
            {
                context.User = principal;
            }
        }
        
        await next(context);
    }
}