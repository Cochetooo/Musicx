using Musicx.Contracts.Dto.Responses;

namespace Musicx.Application.Web.Interfaces.UseCases.User.AlbumAttribute;

public interface IFindAlbumAttributesByAlbumUserService
{
    Task<OutUserAlbumAttribute?> ExecuteAsync(
        long userId,
        long albumId);
    
    OutUserAlbumAttribute? Execute(long userId, long albumId);
}