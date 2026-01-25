using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests.Album;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;

namespace Musicx.Application.Web.Interfaces.UseCases.Specifics;

public interface IGetAlbumByArtistService
{
    Task<OutAlbumList> ExecuteAsync(
        long artistId, 
        IJoinSpecification<InAlbum>? joins = null,
        OrderSpecification<InAlbum>? order = null
    );
    
    OutAlbumList Execute(
        long artistId, 
        IJoinSpecification<InAlbum>? joins = null,
        OrderSpecification<InAlbum>? order = null
    );
}