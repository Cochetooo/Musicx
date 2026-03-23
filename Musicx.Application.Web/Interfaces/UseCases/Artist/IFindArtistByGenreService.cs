using Musicx.Application.Shared.Enums;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Artist;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;

namespace Musicx.Application.Web.Interfaces.UseCases.Artist;

public interface IFindArtistByGenreService
{
    Task<OutGenericList<OutArtist>> ExecuteAsync(
        long genreId, 
        PagingOptions? pagingOptions = null
    );
}