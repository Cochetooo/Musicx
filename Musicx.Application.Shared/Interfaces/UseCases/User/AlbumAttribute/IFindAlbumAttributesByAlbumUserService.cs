using Musicx.Contracts.Dto.Responses;

namespace Musicx.Application.Shared.Interfaces.UseCases.User.AlbumAttribute;

public interface IFindAlbumAttributesByAlbumUserService
{
    Task<OutUserAlbumAttribute?> ExecuteAsync(
        long userId,
        long albumId);
    
    OutUserAlbumAttribute? Execute(long userId, long albumId);
}