using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;

namespace Musicx.Application.Shared.Interfaces.UseCases.User.AlbumAttribute;

public interface IFindAlbumAttributesByUserService
{
    Task<OutGenericList<OutUserAlbumAttribute>> ExecuteAsync(
        long userId, 
        long? artistId = null,
        bool? filterExact = null,
        double? filterSimilitude = 0.4,
        string? filter = null,
        IJoinSpecification<InUserAlbumAttribute>? joins = null,
        OrderSpecification<InUserAlbumAttribute>? order = null,
        PagingOptions? pagingOptions = null,
        CancellationToken cancellationToken = default
    );
    
    OutGenericList<OutUserAlbumAttribute> Execute(
        long userId, 
        long? artistId = null,
        bool? filterExact = null,
        double? filterSimilitude = 0.4,
        string? filter = null,
        IJoinSpecification<InUserAlbumAttribute>? joins = null,
        OrderSpecification<InUserAlbumAttribute>? order = null,
        PagingOptions? pagingOptions = null,
        CancellationToken cancellationToken = default
    );
}