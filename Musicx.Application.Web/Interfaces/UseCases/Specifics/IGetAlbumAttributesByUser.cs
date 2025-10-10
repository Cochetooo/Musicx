using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;

namespace Musicx.Application.Web.Interfaces.UseCases.Specifics;

public interface IGetAlbumAttributesByUser
{
    Task<OutGenericList<OutUserAlbumAttribute>> ExecuteAsync(long userId, 
        long skip = 0, long take = 100, CancellationToken token = default);
    OutGenericList<OutUserAlbumAttribute> Execute(long userId, long skip = 0, long take = 100);
}