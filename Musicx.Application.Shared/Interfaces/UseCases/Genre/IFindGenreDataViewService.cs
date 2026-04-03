using Musicx.Contracts.Dto.Responses.Specifics.Genres;

namespace Musicx.Application.Shared.Interfaces.UseCases.Genre;

public interface IFindGenreDataViewService
{
    Task<OutGenreDataView?> ExecuteAsync(long genreId, long? userId = null);
}