using Musicx.Contracts.Dto.Responses.Specifics.Artists;

namespace Musicx.Application.Shared.Interfaces.UseCases.Artist;

public interface IFindArtistDataViewService
{
    Task<OutArtistDataView?> ExecuteAsync(long artistId, long? userId = null);
}