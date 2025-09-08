namespace Musicx.Application.Api.Interfaces.Auth;

public interface IPasswordHasher
{
    (string HashBase64, string SaltBase64) HashPassword(string password);
    bool VerifyPassword(string password, string storedHashBase64, string storedSaltBase64);
}