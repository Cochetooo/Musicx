using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.User;

namespace Musicx.Application.Api.Interfaces.Auth;

/// <summary>
/// Defines authentication-related operations such as password hashing and user session management.
/// </summary>
/// <remarks>
/// This service abstracts the authentication mechanism and should not expose implementation details
/// such as hashing algorithms or token strategies.
/// </remarks>
/// <since>0.6.5</since>
public interface IAuthService
{
    /// <summary>
    /// Generates and assigns a secure password hash and salt to the given user instance.
    /// </summary>
    /// <param name="rawUser">
    /// The user entity containing the raw (plaintext) password to be hashed.
    /// The password field is expected to be populated prior to calling this method.
    /// </param>
    /// <remarks>
    /// This method mutates the provided user instance by replacing the raw password
    /// with its hashed representation and associated metadata.
    /// </remarks>
    /// <since>0.6.5</since>
    void CreatePasswordHash(ref InUser rawUser);
    
    /// <summary>
    /// Authenticates a user using their credentials and returns an authentication token.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="password">The user's plaintext password.</param>
    /// <returns>
    /// An authentication token representing the authenticated session.
    /// </returns>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown when the provided credentials are invalid.
    /// </exception>
    /// <since>0.6.5</since>
    Task<string> SignInAsync(string email, string password);
    
    /// <summary>
    /// Terminates the current authenticated session.
    /// </summary>
    /// <remarks>
    /// Depending on the implementation, this may revoke tokens, clear cookies,
    /// or invalidate server-side session data.
    /// </remarks>
    /// <since>0.6.5</since>
    Task SignOutAsync();
}