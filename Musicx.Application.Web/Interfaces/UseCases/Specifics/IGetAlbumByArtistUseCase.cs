using Musicx.Contracts.Dto.Responses;

namespace Musicx.Application.Web.Interfaces.UseCases.Specifics;

public interface IGetAlbumByArtistUseCase
{
    Task<List<OutAlbum>> ExecuteAsync(long artistId);
    List<OutAlbum> Execute(long artistId);
}