using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests.Album;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;

namespace Musicx.Application.Web.Interfaces.UseCases.Specifics;

public interface IGetAlbumByArtistUseCase
{
    Task<OutAlbumList> ExecuteAsync(
        long artistId, 
        IJoinSpecification<InAlbum>? joinSpec = null,
        OrderSpecification<InAlbum>? orderSpecification = null
    );
    
    OutAlbumList Execute(
        long artistId, 
        IJoinSpecification<InAlbum>? joinSpec = null,
        OrderSpecification<InAlbum>? orderSpecification = null
    );
}