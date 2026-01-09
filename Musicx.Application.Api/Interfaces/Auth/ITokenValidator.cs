using System.Security.Claims;

namespace Musicx.Application.Api.Interfaces.Auth;

/// <summary>
/// Validates authentication tokens and extracts the associated claims principal.
/// </summary>
/// <remarks>
/// This abstraction encapsulates the verification strategy (e.g. JWT signature,
/// expiration, issuer, audience) and ensures that only valid, untampered tokens
/// produce a <see cref="ClaimsPrincipal"/>.  
/// Implementations must resist tampering and replay attacks.
/// </remarks>
/// <since>0.6.5</since>
public interface ITokenValidator
{
    /// <summary>
    /// Validates the provided authentication token.
    /// </summary>
    /// <param name="token">
    /// The serialized authentication token to validate.
    /// </param>
    /// <returns>
    /// A <see cref="ClaimsPrincipal"/> representing the authenticated identity if the token is valid;
    /// otherwise, <c>null</c>.
    /// </returns>
    /// <remarks>
    /// Validation typically includes signature verification, expiration checks,
    /// and optionally audience/issuer validation depending on implementation.
    /// </remarks>
    /// <since>0.6.5</since>
    ClaimsPrincipal? ValidateToken(string token);
}