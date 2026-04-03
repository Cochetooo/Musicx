using Musicx.Application.Desktop.Interfaces.Library;
using Musicx.Application.Desktop.Models;

namespace Musicx.Application.Desktop.UseCases.Library;

public interface IUpsertLibraryProfileUseCase
{
    Task<LibraryProfile> ExecuteAsync(
        string profileName,
        IReadOnlyList<string> folderPaths,
        bool autoScanOnStartup,
        bool autoHydrateMetadata,
        CancellationToken cancellationToken = default);
}

public sealed class UpsertLibraryProfileUseCase(ILibraryProfileRepository repository) : IUpsertLibraryProfileUseCase
{
    public Task<LibraryProfile> ExecuteAsync(
        string profileName,
        IReadOnlyList<string> folderPaths,
        bool autoScanOnStartup,
        bool autoHydrateMetadata,
        CancellationToken cancellationToken = default)
    {
        var profile = new LibraryProfile {
            Id = Guid.NewGuid(),
            Name = profileName,
            FolderPaths = folderPaths,
            AutoScanOnStartup = autoScanOnStartup,
            AutoHydrateMetadata = autoHydrateMetadata
        };

        return repository.UpsertAsync(profile, cancellationToken);
    }
}