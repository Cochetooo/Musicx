namespace Musicx.Application.Api.Interfaces.Auth;

/// <summary>
/// Provides password hashing and verification capabilities using a secure cryptographic strategy.
/// </summary>
/// <remarks>
/// This interface abstracts the underlying hashing algorithm and encoding format.
/// Implementations must ensure resistance against common attacks
/// (e.g. rainbow tables, brute-force).
/// </remarks>
/// <since>0.6.5</since>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes a plaintext password and generates an associated cryptographic salt.
    /// </summary>
    /// <param name="password">
    /// The plaintext password to hash.
    /// </param>
    /// <returns>
    /// A tuple containing the Base64-encoded hash and salt.
    /// </returns>
    /// <remarks>
    /// The returned values are intended to be persisted as-is and reused for verification.
    /// </remarks>
    /// <since>0.6.5</since>
    (string HashBase64, string SaltBase64) HashPassword(string password);
    
    /// <summary>
    /// Verifies a plaintext password against a stored hash and salt.
    /// </summary>
    /// <param name="password">
    /// The plaintext password to verify.
    /// </param>
    /// <param name="storedHashBase64">
    /// The previously stored Base64-encoded password hash.
    /// </param>
    /// <param name="storedSaltBase64">
    /// The previously stored Base64-encoded salt.
    /// </param>
    /// <returns>
    /// <c>true</c> if the password matches the stored hash; otherwise, <c>false</c>.
    /// </returns>
    /// <since>0.6.5</since>
    bool VerifyPassword(string password, string storedHashBase64, string storedSaltBase64);
}