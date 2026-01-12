using Musicx.Contracts.Dto.Responses;

namespace Musicx.Application.Web.Interfaces.UseCases.Specifics;

public interface IGetAlbumAttributeByAlbumUser
{
    Task<OutUserAlbumAttribute?> ExecuteAsync(
        long userId,
        long albumId);
    
    OutUserAlbumAttribute? Execute(long userId, long albumId);
}