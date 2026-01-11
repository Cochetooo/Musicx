

using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Album;
using Musicx.Contracts.Dto.Requests.Artist;
using Musicx.Contracts.Dto.Requests.Song;
using Musicx.Contracts.Dto.Responses;

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
    /// <since>0.6.1</since>
    Task PersistBatchAsync(
        IEnumerable<InSong> songs,
        IEnumerable<InAlbum> albums,
        IEnumerable<InArtist> artists);
}