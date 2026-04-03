using Musicx.Application.Desktop.Models;

namespace Musicx.Application.Desktop.Interfaces.Library;

public interface ILibraryProfileRepository
{
    Task<LibraryProfile?> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<LibraryProfile> UpsertAsync(LibraryProfile profile, CancellationToken cancellationToken = default);
}