using Musicx.Contracts.Dto.Responses;

namespace Musicx.Application.Web.Interfaces.UseCases.Specifics;

public interface IGetSongByAlbumUseCase
{
    Task<List<OutSong>> ExecuteAsync(long albumId, string query = "");
    List<OutSong> Execute(long albumId, string query = "");
}