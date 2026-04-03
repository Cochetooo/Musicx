using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.User;

namespace Musicx.Application.Shared.Interfaces.UseCases.User.AlbumAttribute;

public interface IFindAlbumAttributesByAlbumUserService
{
    Task<OutUserAlbumAttribute?> ExecuteAsync(
        long userId,
        long albumId);
    
    OutUserAlbumAttribute? Execute(long userId, long albumId);
}