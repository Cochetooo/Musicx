namespace Musicx.Application.Api.Interfaces.Auth;

public interface IAuthService
{
    Task<string> SignInAsync(string email, string password);
    Task SignOutAsync();
}