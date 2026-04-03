namespace Musicx.Application.Shared.Interfaces.UseCases.User.Avatar;

public interface ISaveAvatarService
{
    Task<string> ExecuteAsync(Stream fileStream, string contentType);
    string Execute(Stream fileStream, string contentType);
}