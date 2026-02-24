namespace Musicx.Application.Web.Interfaces.UseCases.User.AlbumAttribute;

public interface IDeleteAlbumAttrByAlbumUserService
{
    Task ExecuteAsync(long userId, long albumId);
}