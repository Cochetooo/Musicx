using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Interfaces.Providers.ExternalMusicData;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Infrastructure.Shared.Providers.ExternalMusicData;

public sealed class CompositeExternalMusicDataProvider(
    IEnumerable<IExternalMusicDataProvider> providers,
    ILoggerProvider loggerProvider) : IExternalMusicDataProvider
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(CompositeExternalMusicDataProvider));
    
    public async Task<OutArtist?> GetArtistInfoAsync(string name, CancellationToken ct = default)
    {
        foreach (var provider in providers)
        {
            try
            {
                var result = await provider.GetArtistInfoAsync(name, ct);
                if (null != result)
                {
                    return result;
                }
            }
            catch
            {
                _logger.LogError("❌ Get Artist Info : An error occured during search.");
                return null;
            }
        }

        _logger.LogWarning($"⚠️ Get Artist Info : Null response returned, not a single api provider found results for: {name}");
        return null;
    }
}