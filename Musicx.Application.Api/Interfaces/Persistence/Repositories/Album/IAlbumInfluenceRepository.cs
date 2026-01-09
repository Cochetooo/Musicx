using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Application.Api.Interfaces.Persistence.Repositories.Album;

/// <summary>
/// Provides persistence operations for album–influence associations.
/// </summary>
/// <remarks>
/// This repository handles the relationship between albums and influence genres,
/// including user-specific tagging or attribution logic when applicable.
/// It extends the generic <see cref="IRepository{TIn, TOut}"/> contract
/// with album–influence–specific query capabilities.
/// </remarks>
/// <since>0.7.0</since>
public interface IAlbumInfluenceRepository : IRepository<InAlbumInfluence, OutAlbumInfluence>
{
    /// <summary>
    /// Retrieves a specific album–influence association for a given album, influence genre, and tagger.
    /// </summary>
    /// <param name="albumId">
    /// The unique identifier of the album.
    /// </param>
    /// <param name="genreId">
    /// The unique identifier of the influence genre.
    /// </param>
    /// <param name="taggerId">
    /// The unique identifier of the user who tagged or associated the influence genre.
    /// </param>
    /// <returns>
    /// The matching album–influence association if found; otherwise, <c>null</c>.
    /// </returns>
    /// <since>0.7.0</since>
    Task<OutAlbumInfluence?> FindOneAsync(long albumId, long genreId, long taggerId);
}