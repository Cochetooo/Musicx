namespace Musicx.Contracts.Dto.Requests;

public sealed class InRelease : BaseInputModel
{
    // Required Relationships
    public long AlbumId { get; set; }
    
    // Required Columns
    public string CatalogNumber { get; set; } = null!;

    // Optional Relationships
    public long? LabelId { get; set; }
    
    // Optional Columns
    public DateTime? ReleaseDate { get; set; }
}