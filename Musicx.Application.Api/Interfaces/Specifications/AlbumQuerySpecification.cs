using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;


namespace Musicx.Application.Api.Interfaces.Specifications;

/// <summary>
/// Specify the relation to include when retrieving album from repository.
/// </summary>
/// <since>0.6.1</since>
public record AlbumQuerySpecification : IQuerySpecification<InAlbum>
{
    /// <summary>
    /// Include the artist of this album.
    /// </summary>
    /// <since>0.6.1</since>
    public bool IncludeArtist { get; set; }
    
    /// <summary>
    /// Include all release issues of this album.
    /// </summary>
    /// <since>0.6.1</since>
    public bool IncludeReleases { get; set; }
    
    /// <summary>
    /// Include primary genres of this album.
    /// </summary>
    /// <since>0.6.1</since>
    public bool IncludePrimaryGenres { get; set; }
    
    /// <summary>
    /// Include influence genres of this album.
    /// </summary>
    /// <since>0.6.1</since>
    public bool IncludeInfluenceGenres { get; set; }
}