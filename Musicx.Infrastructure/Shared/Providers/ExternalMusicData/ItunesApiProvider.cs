using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Interfaces.Providers.ExternalMusicData;
using Musicx.Contracts.Dto.Responses;
using Musicx.Infrastructure.Shared.Models.ExternalMusicData;

namespace Musicx.Infrastructure.Shared.Providers.ExternalMusicData;

public sealed class ItunesApiProvider(
    HttpClient httpClient,
    ILoggerProvider loggerProvider,
    IConfiguration configuration) : IExternalMusicDataProvider
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(ItunesApiProvider));

    public async Task<OutArtist?> GetArtistInfoAsync(string name, CancellationToken ct = default)
        => null;
}