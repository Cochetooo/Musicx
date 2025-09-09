using Musicx.Contracts.Dto.Requests;

namespace Musicx.Application.Api.Interfaces.Auth;

public interface IAuthService
{
    void CreatePasswordHash(ref InUser rawUser);
    Task<string> SignInAsync(string email, string password);
    Task SignOutAsync();
}