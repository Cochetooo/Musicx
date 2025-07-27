using Musicx.Contracts.Dto.Responses;

namespace Musicx.Application.Web.Interfaces.UseCases.Specifics;

public interface IGetAlbumByGenreUseCase
{
    Task<List<OutAlbum>> ExecuteAsync(long genreId, 
        int genreOptions,
        int skip = 0,
        int take = 100,
        string query = "");
    Task<List<OutAlbum>> ExecuteAsync(OutGenre genre, int genreOptions,
        int skip = 0,
        int take = 100,
        string query = "");
    
    List<OutAlbum> Execute(long genreId, int genreOptions,
        int skip = 0,
        int take = 100,
        string query = "");
    List<OutAlbum> Execute(OutGenre genre, int genreOptions,
        int skip = 0,
        int take = 100,
        string query = "");
}