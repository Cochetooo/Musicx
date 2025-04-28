using Musicx.Domain.Models;

namespace Musicx.Application.Desktop.Interfaces.Persistence;

public interface IBatchImportRepository
{
    Task PersistBatchAsync(
        IEnumerable<Song> songs,
        IEnumerable<Album> albums,
        IEnumerable<Artist> artists);
}