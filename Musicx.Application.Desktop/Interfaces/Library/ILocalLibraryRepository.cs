using Musicx.Application.Desktop.Models;

namespace Musicx.Application.Desktop.Interfaces.Library;

public interface ILocalLibraryRepository
{
    Task UpsertTracksAsync(IReadOnlyCollection<LocalTrack> tracks, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LocalTrack>> GetTracksAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetArtistsAsync(CancellationToken cancellationToken = default);
}