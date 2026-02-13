namespace Musicx.Application.Web.Interfaces.UseCases.User.Avatar;

public interface ISaveAvatarService
{
    Task<string> ExecuteAsync(Stream fileStream, string contentType);
    string Execute(Stream fileStream, string contentType);
}