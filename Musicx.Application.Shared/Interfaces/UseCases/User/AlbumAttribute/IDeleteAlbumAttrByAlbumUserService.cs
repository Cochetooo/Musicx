namespace Musicx.Application.Shared.Interfaces.UseCases.User.AlbumAttribute;

public interface IDeleteAlbumAttrByAlbumUserService
{
    Task ExecuteAsync(long userId, long albumId);
}