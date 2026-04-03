using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;

namespace Musicx.Application.Shared.Interfaces.UseCases.User.AlbumAttribute;

public interface IFindAlbumAttributesByAlbumService
{
    Task<OutGenericList<OutUserAlbumAttribute>> ExecuteAsync(
        long albumId, 
        OrderSpecification<InUserAlbumAttribute>? order = null,
        PagingOptions? pagingOptions = null, 
        CancellationToken cancellationToken = default
    );
    
    OutGenericList<OutUserAlbumAttribute> Execute(
        long albumId, 
        OrderSpecification<InUserAlbumAttribute>? order = null,
        PagingOptions? pagingOptions = null, 
        CancellationToken cancellationToken = default
    );
}