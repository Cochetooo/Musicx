using Musicx.Contracts.Dto.Requests.Specifics;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;

namespace Musicx.Application.Web.Interfaces.UseCases.Album;

public interface IFindAlbumByChartService
{
    Task<OutAlbumList> ExecuteAsync(AlbumChartQuery query);
    OutAlbumList Execute(AlbumChartQuery query);
}