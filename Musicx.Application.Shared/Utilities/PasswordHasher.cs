using System.Security.Cryptography;

namespace Musicx.Application.Shared.Utilities;

public static class PasswordHasher
{
    private const int SaltBytes = 32;
    private const int KeyBytes = 32;
    private const int Iterations = 200_000;

    public static (string HashBase64, string SaltBase64) HashPassword(string password)
    {
        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[SaltBytes];
        rng.GetBytes(salt);
        
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        var key = pbkdf2.GetBytes(KeyBytes);
        
        return (Convert.ToBase64String(key), Convert.ToBase64String(salt));
    }

    public static bool VerifyPassword(string password, string storedHashBase64, string storedSaltBase64)
    {
        var salt = Convert.FromBase64String(storedSaltBase64);
        var storedHash = Convert.FromBase64String(storedHashBase64);
        
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        var computed = pbkdf2.GetBytes(KeyBytes);
        
        return CryptographicOperations.FixedTimeEquals(storedHash, computed);
    }
}