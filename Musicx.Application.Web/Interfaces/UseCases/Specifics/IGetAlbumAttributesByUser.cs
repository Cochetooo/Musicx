using Musicx.Contracts.Dto.Responses;

namespace Musicx.Application.Web.Interfaces.UseCases.Specifics;

public interface IGetAlbumAttributesByUser
{
    Task<List<OutUserAlbumAttribute>> ExecuteAsync(long userId);
    List<OutUserAlbumAttribute> Execute(long userId);
}