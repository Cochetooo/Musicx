using Musicx.Application.Api.Interfaces.Auth;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.User;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Infrastructure.API.Persistence.Specifications.User;

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
        var userAuth = await userRepository.FindAuthByEmailAsync(email, new UserJoinSpecification
        {
            IncludeRoles = true
        });
        
        if (userAuth is null)
        {
            throw new UnauthorizedAccessException("Invalid credentials : User not found");
        }

        if (userAuth.PasswordHash is null || userAuth.PasswordSalt is null)
        {
            throw new UnauthorizedAccessException("Invalid credentials : Password not stored");
        }

        if (!passwordHasher.VerifyPassword(password, userAuth.PasswordHash, userAuth.PasswordSalt))
        {
            throw new UnauthorizedAccessException("Invalid credentials : Password not correct");
        }

        return tokenGenerator.Generate(userAuth.User);
    }
    
    // Côté client, supprimer le cookie / token
    public Task SignOutAsync()
        => Task.CompletedTask;
}