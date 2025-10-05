using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;

namespace Musicx.Application.Web.Interfaces.UseCases.Specifics;

public interface IGetAlbumAttributesByAlbum
{
    Task<OutGenericList<OutUserAlbumAttribute>> ExecuteAsync(long albumId, 
        int skip = 0, int take = 100, CancellationToken cancellationToken = default);
    OutGenericList<OutUserAlbumAttribute> Execute(long albumId, int skip = 0, int take = 100);
}