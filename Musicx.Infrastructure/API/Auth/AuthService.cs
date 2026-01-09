using Musicx.Application.Api.Interfaces.Auth;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.User;
using Musicx.Application.Api.Interfaces.Specifications;
using Musicx.Contracts.Dto.Requests;

namespace Musicx.Infrastructure.API.Auth;

public sealed class AuthService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenGenerator tokenGenerator) : IAuthService
{
    public void CreatePasswordHash(ref InUser rawUser)
    {
        if (rawUser.Password is null)
        {
            throw new NullReferenceException("User Password is null");
        }
        
        var (hash, salt) = passwordHasher.HashPassword(rawUser.Password);
        
        rawUser.PasswordHash = hash;
        rawUser.PasswordSalt = salt;
        rawUser.Password = null;
    }

    public async Task<string> SignInAsync(string email, string password)
    {
        var user = await userRepository.FindByEmailAsync(email, new UserQuerySpecification
        {
            IncludeRoles = true
        });
        
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