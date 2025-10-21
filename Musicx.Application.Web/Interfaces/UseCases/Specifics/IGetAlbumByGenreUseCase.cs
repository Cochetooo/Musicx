using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;

namespace Musicx.Application.Web.Interfaces.UseCases.Specifics;

public interface IGetAlbumByGenreUseCase
{
    Task<OutAlbumList> ExecuteAsync(long genreId, 
        int genreOptions,
        long skip = 0,
        long take = 100,
        string order = "",
        string query = "");
    Task<OutAlbumList> ExecuteAsync(OutGenre genre, int genreOptions,
        long skip = 0,
        long take = 100,
        string order = "",
        string query = "");
    
    OutAlbumList Execute(long genreId, int genreOptions,
        long skip = 0,
        long take = 100,
        string order = "",
        string query = "");
    OutAlbumList Execute(OutGenre genre, int genreOptions,
        long skip = 0,
        long take = 100,
        string order = "",
        string query = "");
}