namespace Musicx.Application.Web.Interfaces.UseCases.Specifics;

public interface IDeleteFavouriteAlbumService
{
    Task ExecuteAsync(long userId, long albumId);
    void Execute(long userId, long albumId);
}