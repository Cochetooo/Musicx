namespace Musicx.Application.Web.Interfaces.UseCases.Security;

public interface IAuthUserSavePasswordService
{
    Task ExecuteAsync(long userId, string password);
}