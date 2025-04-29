using Musicx.Domain.Models;

namespace Musicx.Application.Desktop.Interfaces.Persistence;

/// <summary>
/// Repository for batch file import.
/// </summary>
/// <since>0.6.1</since>
public interface IBatchImportRepository
{
    /// <summary>
    /// Persist a batch of songs into the database.
    /// </summary>
    /// <param name="songs">List of songs retrieved</param>
    /// <param name="albums">List of albums retrieved</param>
    /// <param name="artists">List of artists retrieved</param>
    /// <returns></returns>
    Task PersistBatchAsync(
        IEnumerable<Song> songs,
        IEnumerable<Album> albums,
        IEnumerable<Artist> artists);
}