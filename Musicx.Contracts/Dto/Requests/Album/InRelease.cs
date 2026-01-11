namespace Musicx.Contracts.Dto.Requests.Album;

public sealed class InRelease : BaseInputModel
{
    // Required Relationships
    public long AlbumId { get; set; }
    
    // Required Columns
    public string CatalogNumber { get; set; } = null!;
    public bool IsVisible { get; set; }

    // Optional Relationships
    public long? LabelId { get; set; }
    
    // Optional Columns
    public DateTime? ReleaseDate { get; set; }
}