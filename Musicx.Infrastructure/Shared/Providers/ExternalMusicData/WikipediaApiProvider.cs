using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Helpers;
using Musicx.Application.Shared.Interfaces.Providers.ExternalMusicData;
using Musicx.Contracts.Dto.Responses.Specifics.Artwork;

namespace Musicx.Infrastructure.Shared.Providers.ExternalMusicData;

public sealed class WikipediaApiProvider(
    HttpClient httpClient,
    ILoggerProvider loggerProvider,
    IConfiguration configuration) : IExternalMusicDataProvider
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(WikipediaApiProvider));

    public async Task<OutArtworkCandidate?> GetArtistArtworkAsync(string name, string normalizedName,
            CancellationToken ct = default)
        /* {
            var searchEndpoint = $"https://en.wikipedia.org/w/api.php?action=query&list=search&srsearch={Uri.EscapeDataString(name + " musician")}&utf8=1&format=json";
            var searchJson = await httpClient.GetStringAsync(searchEndpoint, ct);
            using var searchDocument = JsonDocument.Parse(searchJson);

            var pageTitle = searchDocument.RootElement
                .GetProperty("query")
                .GetProperty("search")
                .EnumerateArray()
                .Select(item => item.GetProperty("title").GetString())
                .FirstOrDefault(title => !string.IsNullOrWhiteSpace(title));

            if (string.IsNullOrWhiteSpace(pageTitle))
            {
                return null;
            }

            var summaryEndpoint = $"https://en.wikipedia.org/api/rest_v1/page/summary/{Uri.EscapeDataString(pageTitle)}";
            var summaryJson = await httpClient.GetStringAsync(summaryEndpoint, ct);
            using var summaryDocument = JsonDocument.Parse(summaryJson);

            if (!summaryDocument.RootElement.TryGetProperty("thumbnail", out var thumbnail) || !thumbnail.TryGetProperty("source", out var source))
            {
                return null;
            }

            var thumbnailUrl = source.GetString();
            if (string.IsNullOrWhiteSpace(thumbnailUrl))
            {
                return null;
            }

            return IExternalMusicDataProvider.BuildCandidate(
                url: thumbnailUrl,
                source: "Wikipedia",
                label: pageTitle,
                width: 640,
                height: 640,
                score: IExternalMusicDataProvider.ComputeScore(normalizedName, StringHelper.Normalize(pageTitle)) + 2
            );
        } */
        => null;

    public async Task<OutArtworkCandidate?> GetAlbumArtworkAsync(string name, string artist, string normalizedName,
        string normalizedArtist,
        CancellationToken ct = default)
        => null;
}