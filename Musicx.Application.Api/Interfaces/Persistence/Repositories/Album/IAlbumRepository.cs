using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Album;
using Musicx.Contracts.Dto.Requests.Specifics;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Application.Api.Interfaces.Persistence.Repositories.Album;

/// <summary>
/// Provides persistence operations for album entities.
/// </summary>
/// <remarks>
/// This repository exposes album-specific query methods in addition to the
/// generic <see cref="IRepository{TIn, TOut}"/> contract.
/// It supports filtered retrieval based on artists, genres, and chart-related criteria.
/// </remarks>
/// <since>0.6.0</since>
public interface IAlbumRepository : IRepository<InAlbum, OutAlbum>
{
    /// <summary>
    /// Returns the total number of albums associated with the specified genre.
    /// </summary>
    /// <param name="genreId">
    /// The unique identifier of the genre.
    /// </param>
    /// <returns>
    /// The number of albums linked to the given genre.
    /// </returns>
    /// <since>0.6.5</since>
    Task<long> GetCountByGenreIdAsync(long genreId);
    
    /// <summary>
    /// Retrieves all albums associated with the specified artist.
    /// </summary>
    /// <param name="artistId">
    /// The unique identifier of the artist.
    /// </param>
    /// <param name="albumQuerySpecification">
    /// An optional query specification used to further filter or customize the album query.
    /// </param>
    /// <returns>
    /// A list of albums linked to the given artist.
    /// </returns>
    /// <since>0.6.3</since>
    Task<List<OutAlbum>> FindByArtistIdAsync(long artistId, 
        IJoinSpecification<InAlbum>? joinSpec = null,
        OrderSpecification<InAlbum>? orderSpecification = null);
    
    Task<Dictionary<long, List<OutAlbum>>> FindByArtistIdsAsync(
        IEnumerable<long> artistIds,
        IJoinSpecification<InAlbum>? joinSpec = null,
        OrderSpecification<InAlbum>? orderSpecification = null);
    
    /// <summary>
    /// Retrieves albums associated with the specified genre.
    /// </summary>
    /// <param name="genreId">
    /// The unique identifier of the genre.
    /// </param>
    /// <param name="genreOptions">
    /// A bitwise combination of genre-related filtering options
    /// (e.g. inclusion of subgenres, influence-based expansion).
    /// </param>
    /// <param name="skip">
    /// The number of albums to skip for pagination purposes.
    /// </param>
    /// <param name="take">
    /// The maximum number of albums to retrieve.
    /// </param>
    /// <param name="order">
    /// An optional ordering expression applied to the result set.
    /// </param>
    /// <param name="albumQuerySpecification">
    /// An optional query specification used to further filter or customize the album query.
    /// </param>
    /// <returns>
    /// A list of albums matching the specified genre and filtering options.
    /// </returns>
    /// <since>0.6.5</since>
    Task<List<OutAlbum>> FindByGenreIdAsync(long genreId, 
        int genreOptions,
        IJoinSpecification<InAlbum>? joinSpec = null,
        OrderSpecification<InAlbum>? orderSpec = null,
        PagingOptions? pagingOptions = null);
    
    /// <summary>
    /// Retrieves albums based on chart-specific criteria.
    /// </summary>
    /// <param name="query">
    /// An object encapsulating chart-related filtering, ranking, and time-range criteria.
    /// </param>
    /// <returns>
    /// A list of albums matching the specified chart query.
    /// </returns>
    /// <since>0.6.7</since>
    Task<List<OutAlbum>> FindByChart(
        AlbumChartQuery query);
}