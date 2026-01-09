using Musicx.Contracts.Dto.Responses;

namespace Musicx.Application.Api.Interfaces.Auth;

/// <summary>
/// Generates authentication tokens for authenticated users.
/// </summary>
/// <remarks>
/// This abstraction encapsulates the token generation strategy
/// (e.g. JWT, opaque tokens) and the embedded user-related claims.
/// Implementations must ensure token integrity, expiration handling,
/// and resistance against tampering.
/// </remarks>
/// <since>0.6.5</since>
public interface ITokenGenerator
{
    /// <summary>
    /// Generates an authentication token for the specified user.
    /// </summary>
    /// <param name="user">
    /// The authenticated user for whom the token is generated.
    /// Only non-sensitive, authorization-relevant data should be embedded.
    /// </param>
    /// <returns>
    /// A serialized authentication token suitable for client consumption.
    /// </returns>
    /// <since>0.6.5</since>
    string Generate(OutUser user);
}