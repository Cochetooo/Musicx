using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Album;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Application.Api.Interfaces.Persistence.Repositories.Album;

/// <summary>
/// Provides persistence operations for album–genre associations.
/// </summary>
/// <remarks>
/// This repository handles the relationship between albums and genres,
/// including user-specific tagging or attribution logic when applicable.
/// It extends the generic <see cref="IRepository{TIn, TOut}"/> contract
/// with album–genre–specific query capabilities.
/// </remarks>
/// <since>0.7.0</since>
public interface IAlbumGenreRepository : IRepository<InAlbumGenre, OutAlbumGenre>
{
    /// <summary>
    /// Retrieves a specific album–genre association for a given album, genre, and tagger.
    /// </summary>
    /// <param name="albumId">
    /// The unique identifier of the album.
    /// </param>
    /// <param name="genreId">
    /// The unique identifier of the genre.
    /// </param>
    /// <param name="taggerId">
    /// The unique identifier of the user who tagged or associated the genre.
    /// </param>
    /// <returns>
    /// The matching album–genre association if found; otherwise, <c>null</c>.
    /// </returns>
    /// <since>0.7.0</since>
    Task<OutAlbumGenre?> FindOneAsync(long albumId, long genreId, long taggerId);
}