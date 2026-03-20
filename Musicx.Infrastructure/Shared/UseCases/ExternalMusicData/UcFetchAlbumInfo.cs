using Musicx.Application.Shared.Interfaces.UseCases.ExternalMusicData;
using Musicx.Infrastructure.Shared.Providers.ExternalMusicData;

namespace Musicx.Infrastructure.Shared.UseCases.ExternalMusicData;

public sealed class UcFetchAlbumInfo(
    ExternalMusicDataService externalMusicDataProvider) : IFetchAlbumInfoClientService
{
    public async Task<FetchAlbumInfoResponse> ExecuteAsync(FetchAlbumInfoRequest request)
    {
        var result = await externalMusicDataProvider.GetAlbumArtworkAsync(
            request.Name,
            request.Artist,
            request.CancellationToken);
        
        return new FetchAlbumInfoResponse(result);
    }

    public FetchAlbumInfoResponse Execute(FetchAlbumInfoRequest request)
    {
        throw new NotImplementedException();
    }
}