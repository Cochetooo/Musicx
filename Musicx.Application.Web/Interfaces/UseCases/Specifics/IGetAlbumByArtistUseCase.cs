using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;

namespace Musicx.Application.Web.Interfaces.UseCases.Specifics;

public interface IGetAlbumByArtistUseCase
{
    Task<OutAlbumList> ExecuteAsync(long artistId, string query = "");
    OutAlbumList Execute(long artistId, string query = "");
}