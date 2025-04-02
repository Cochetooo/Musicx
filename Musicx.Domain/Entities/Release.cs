namespace Musicx.Domain.Entities;

/// <summary>
/// Entity representing a release issue of an album.
/// </summary>
/// <since>0.6.1</since>
public sealed class Release : BaseEntity
{
    /// <summary>
    /// Album of this release.
    /// </summary>
    /// <since>0.6.1</since>
    public required Album Album { get; set; }
    
    /// <summary>
    /// Label that released this issue.
    /// </summary>
    /// <since>0.6.1</since>
    public Label? Label { get; set; }
    
    /// <summary>
    /// Identification number assigned to this music release by the record label.
    /// </summary>
    /// <since>0.6.1</since>
    public string? CatalogNumber { get; set; }
    
    /// <summary>
    /// Release date for this issue.
    /// </summary>
    /// <since>0.6.1</since>
    public DateTime? ReleaseDate { get; set; }
}