using Musicx.Application.Desktop.Interfaces.Library;
using Musicx.Application.Desktop.Models;

namespace Musicx.Infrastructure.Desktop.Services.Library;

public sealed class InMemoryImportProgressPublisher : IImportProgressPublisher
{
    public event Action<ImportProgressSnapshot>? SnapshotPublished;

    public void Publish(ImportProgressSnapshot snapshot)
    {
        SnapshotPublished?.Invoke(snapshot);
    }
}