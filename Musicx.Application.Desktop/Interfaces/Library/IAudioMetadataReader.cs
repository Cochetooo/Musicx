using Musicx.Application.Desktop.Models;

namespace Musicx.Application.Desktop.Interfaces.Library;

public interface IAudioMetadataReader
{
    Task<LocalTrack> ReadAsync(string filePath, CancellationToken cancellationToken = default);
}