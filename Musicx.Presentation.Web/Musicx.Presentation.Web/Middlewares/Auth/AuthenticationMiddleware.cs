using Musicx.Application.Api.Interfaces.Auth;

namespace Musicx.Presentation.Web.Middlewares.Auth;

public sealed class AuthenticationMiddleware(
    RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ITokenValidator tokenValidator)
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Substring("Bearer ".Length).Trim();
            
            var principal = tokenValidator.ValidateToken(token);
            if (principal != null)
            {
                context.User = principal;
            }
        }
        
        await next(context);
    }
}