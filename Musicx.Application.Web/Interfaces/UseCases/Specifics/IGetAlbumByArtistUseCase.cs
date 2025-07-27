using Musicx.Contracts.Dto.Responses;

namespace Musicx.Application.Web.Interfaces.UseCases.Specifics;

public interface IGetAlbumByArtistUseCase
{
    Task<List<OutAlbum>> ExecuteAsync(long artistId, string query = "");
    List<OutAlbum> Execute(long artistId, string query = "");
}