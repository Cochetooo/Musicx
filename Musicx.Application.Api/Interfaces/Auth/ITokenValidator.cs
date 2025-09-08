using System.Security.Claims;

namespace Musicx.Application.Api.Interfaces.Auth;

public interface ITokenValidator
{
    ClaimsPrincipal? ValidateToken(string token);
}