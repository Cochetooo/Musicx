using Musicx.Application.Desktop.Interfaces.Library;
using Musicx.Application.Desktop.Models;

namespace Musicx.Application.Desktop.UseCases.Library;

public interface IGetOrCreateDefaultProfileUseCase
{
    Task<LibraryProfile> ExecuteAsync(CancellationToken cancellationToken = default);
}

public sealed class GetOrCreateDefaultProfileUseCase(ILibraryProfileRepository repository) : IGetOrCreateDefaultProfileUseCase
{
    public async Task<LibraryProfile> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var existing = await repository.GetActiveAsync(cancellationToken);
        if (existing is not null)
        {
            return existing;
        }

        var defaultMusicPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        var profile = new LibraryProfile
        {
            Id = Guid.NewGuid(),
            Name = "Default",
            FolderPaths = string.IsNullOrWhiteSpace(defaultMusicPath) ? [] : [defaultMusicPath],
            AutoScanOnStartup = true,
            AutoHydrateMetadata = true
        };

        return await repository.UpsertAsync(profile, cancellationToken);
    }
}