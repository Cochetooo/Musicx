namespace Musicx.Contracts.Dto.Requests;

public sealed class InRelease : BaseModel
{
    public long AlbumId { get; set; }
    public long? LabelId { get; set; }
    
    public string? CatalogNumber { get; set; }
    public DateTime? ReleaseDate { get; set; }
}