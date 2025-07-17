using Musicx.Contracts.Dto.Responses;

namespace Musicx.Application.Shared.Interfaces.Providers.ExternalMusicData;

public interface IExternalMusicDataProvider
{
    Task<OutArtist?> GetArtistInfoAsync(string name, CancellationToken ct = default);
}