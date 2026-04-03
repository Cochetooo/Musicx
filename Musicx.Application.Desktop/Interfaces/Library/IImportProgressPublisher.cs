using Musicx.Application.Desktop.Models;

namespace Musicx.Application.Desktop.Interfaces.Library;

public interface IImportProgressPublisher
{
    void Publish(ImportProgressSnapshot snapshot);
}