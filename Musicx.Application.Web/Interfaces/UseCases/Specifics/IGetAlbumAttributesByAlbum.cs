using Musicx.Contracts.Dto.Responses;

namespace Musicx.Application.Web.Interfaces.UseCases.Specifics;

public interface IGetAlbumAttributesByAlbum
{
    Task<List<OutUserAlbumAttribute>> ExecuteAsync(long albumId);
    List<OutUserAlbumAttribute> Execute(long albumId);
}