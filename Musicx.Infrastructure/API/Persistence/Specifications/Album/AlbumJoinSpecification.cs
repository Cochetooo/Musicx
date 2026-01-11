using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Album;

namespace Musicx.Infrastructure.API.Persistence.Specifications.Album;

/// <summary>
/// Specify the relation to include when retrieving album from repository.
/// </summary>
/// <since>0.6.1</since>
public record AlbumJoinSpecification : IJoinSpecification<InAlbum>
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
    
    /// <summary>
    /// Include stats of this album.
    /// </summary>
    /// <since>0.6.6</since>
    public bool IncludeStats { get; set; }
    
    public override string ToString() 
        => $"Artist: {IncludeArtist} | Releases: {IncludeReleases} | PrimaryGenres: {IncludePrimaryGenres} " +
           $"| InfluenceGenres: {IncludeInfluenceGenres} | Stats: {IncludeStats}";
}