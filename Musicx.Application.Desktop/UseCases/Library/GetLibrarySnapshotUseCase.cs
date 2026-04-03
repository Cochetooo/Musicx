using Musicx.Application.Desktop.Interfaces.Library;
using Musicx.Application.Desktop.Models;

namespace Musicx.Application.Desktop.UseCases.Library;

public interface IGetLibrarySnapshotUseCase
{
    Task<LibrarySnapshot> ExecuteAsync(CancellationToken cancellationToken = default);
}

public sealed record LibrarySnapshot(IReadOnlyList<string> Artists, IReadOnlyList<LocalTrack> Tracks);

public sealed class GetLibrarySnapshotUseCase(ILocalLibraryRepository repository) : IGetLibrarySnapshotUseCase
{
    public async Task<LibrarySnapshot> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var artists = await repository.GetArtistsAsync(cancellationToken);
        var tracks = await repository.GetTracksAsync(cancellationToken);
        return new LibrarySnapshot(artists, tracks);
    }
}