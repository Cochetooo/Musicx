namespace Musicx.Application.Shared.Interfaces.UseCases.User.FavoriteAlbum;

public interface IDeleteFavouriteAlbumService
{
    Task ExecuteAsync(long userId, long albumId);
    void Execute(long userId, long albumId);
}