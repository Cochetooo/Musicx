namespace Musicx.Contracts.Dto.Responses;

public sealed class OutRelease
{
    public long Id { get; set; }
    
    public OutAlbum Album { get; set; } = null!;
    public OutLabel? Label { get; set; }

    public string CatalogNumber { get; set; } = null!;
    public DateTime? ReleaseDate { get; set; }
}