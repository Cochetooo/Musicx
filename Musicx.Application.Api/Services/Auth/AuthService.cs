using Musicx.Application.Api.Interfaces.Auth;
using Musicx.Application.Api.Interfaces.Persistence;

namespace Musicx.Application.Api.Services.Auth;

public sealed class AuthService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenGenerator tokenGenerator) : IAuthService
{
    public async Task<string> SignInAsync(string email, string password)
    {
        var user = await userRepository.FindByEmailAsync(email);
        if (user is null)
        {
            throw new UnauthorizedAccessException("Invalid credentials : User not found");
        }

        if (user.PasswordHash is null || user.PasswordSalt is null)
        {
            throw new UnauthorizedAccessException("Invalid credentials : Password not stored");
        }

        if (!passwordHasher.VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
        {
            throw new UnauthorizedAccessException("Invalid credentials : Password not correct");
        }

        return tokenGenerator.Generate(user);
    }
    
    // Côté client, supprimer le cookie / token
    public Task SignOutAsync()
        => Task.CompletedTask;
}