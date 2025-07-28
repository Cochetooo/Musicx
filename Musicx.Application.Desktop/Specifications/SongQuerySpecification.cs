using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;


namespace Musicx.Application.Desktop.Specifications;

/// <summary>
/// Specify the relation to include when retrieving song from repository.
/// </summary>
/// <since>0.6.1</since>
public record SongQuerySpecification : IQuerySpecification<InSong>
{
    /// <summary>
    /// Include the artist of this song.
    /// </summary>
    /// <since>0.6.1</since>
    public bool IncludeArtist { get; set; }
    
    /// <summary>
    /// Include the album of this song.
    /// </summary>
    /// <since>0.6.1</since>
    public bool IncludeAlbum { get; set; }
    
    /// <summary>
    /// Include the artist of the album of this song.
    /// </summary>
    /// <since>0.6.1</since>
    public bool IncludeAlbumArtist { get; set; }
    
    /// <summary>
    /// Include primary genres of this song.
    /// </summary>
    /// <since>0.6.1</since>
    public bool IncludePrimaryGenres { get; set; }
    
    /// <summary>
    /// Include influence genres of this song.
    /// </summary>
    /// <since>0.6.1</since>
    public bool IncludeInfluenceGenres { get; set; }
}