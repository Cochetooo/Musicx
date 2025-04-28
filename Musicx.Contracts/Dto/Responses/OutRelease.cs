namespace Musicx.Contracts.Dto.Responses;

public sealed class OutRelease : BaseModel
{
    public OutAlbum Album { get; set; } = null!;
    public OutLabel? Label { get; set; }
    
    public string? CatalogNumber { get; set; }
    public DateTime? ReleaseDate { get; set; }
}