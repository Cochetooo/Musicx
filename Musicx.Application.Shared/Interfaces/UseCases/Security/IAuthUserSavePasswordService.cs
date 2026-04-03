namespace Musicx.Application.Shared.Interfaces.UseCases.Security;

public interface IAuthUserSavePasswordService
{
    Task ExecuteAsync(long userId, string password);
}