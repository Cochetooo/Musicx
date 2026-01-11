using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;

namespace Musicx.Application.Web.Interfaces.UseCases.Specifics;

public interface IGetAlbumAttributesByAlbum
{
    Task<OutGenericList<OutUserAlbumAttribute>> ExecuteAsync(
        long albumId, 
        OrderSpecification<InUserAlbumAttribute>? orderSpec = null,
        PagingOptions? pagingOptions = null, 
        CancellationToken cancellationToken = default
    );
    
    OutGenericList<OutUserAlbumAttribute> Execute(
        long albumId, 
        OrderSpecification<InUserAlbumAttribute>? orderSpec = null,
        PagingOptions? pagingOptions = null, 
        CancellationToken cancellationToken = default
    );
}