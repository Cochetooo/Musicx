namespace Musicx.Contracts.Dto.Responses;

public sealed class OutRelease : BaseOutputModel
{
    public OutAlbum Album { get; set; } = null!;
    public OutLabel? Label { get; set; }

    public string CatalogNumber { get; set; } = null!;
    public DateTime? ReleaseDate { get; set; }
}