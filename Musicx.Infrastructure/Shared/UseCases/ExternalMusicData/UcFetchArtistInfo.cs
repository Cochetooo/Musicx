using Musicx.Application.Shared.Interfaces.Providers.ExternalMusicData;
using Musicx.Application.Shared.Interfaces.UseCases.ExternalMusicData;

namespace Musicx.Infrastructure.Shared.UseCases.ExternalMusicData;

public sealed class UcFetchArtistInfo(
    IExternalMusicDataProvider externalMusicDataProvider) : IFetchArtistInfoUseCase
{
    public async Task<FetchArtistInfoResponse> ExecuteAsync(FetchArtistInfoRequest request)
    {
        var result = await externalMusicDataProvider.GetArtistInfoAsync(request.Name, request.CancellationToken);

        return new FetchArtistInfoResponse
        (
            Artist: result
        );
    }

    public FetchArtistInfoResponse Execute(FetchArtistInfoRequest request)
    {
        throw new NotImplementedException();
    }
}