using ATL;
using Musicx.Application.Desktop.Interfaces.Library;
using Musicx.Application.Desktop.Models;

namespace Musicx.Infrastructure.Desktop.Services.Library;

public sealed class AtlAudioMetadataReader : IAudioMetadataReader
{
    public Task<LocalTrack> ReadAsync(string filePath, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var track = new Track(filePath);

        var model = new LocalTrack {
            Id = Guid.NewGuid(),
            FilePath = filePath,
            Title = string.IsNullOrWhiteSpace(track.Title) ? Path.GetFileNameWithoutExtension(filePath) : track.Title,
            Artist = string.IsNullOrWhiteSpace(track.Artist) ? "Unknown Artist" : track.Artist,
            Album = string.IsNullOrWhiteSpace(track.Album) ? "Unknown Album" : track.Album,
            Format = Path.GetExtension(filePath).TrimStart('.').ToLowerInvariant(),
            Duration = track.Duration <= 0 ? TimeSpan.Zero : TimeSpan.FromSeconds(track.Duration),
            ApiSongId = null,
            ApiAlbumId = null,
            ApiArtistId = null,
            IsMatchedWithApi = false
        };

        return Task.FromResult(model);
    }
}