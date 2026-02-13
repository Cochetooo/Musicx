using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests.Album;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;

namespace Musicx.Application.Web.Interfaces.UseCases.Album;

public interface IFindAlbumByGenreService
{
    Task<OutAlbumList> ExecuteAsync(
        long genreId, 
        int genreOptions,
        IJoinSpecification<InAlbum>? joins = null,
        OrderSpecification<InAlbum>? order = null,
        PagingOptions? pagingOptions = null
    );
    
    OutAlbumList Execute(
        long genreId, 
        int genreOptions,
        IJoinSpecification<InAlbum>? joins = null,
        OrderSpecification<InAlbum>? order = null,
        PagingOptions? pagingOptions = null
    );
}