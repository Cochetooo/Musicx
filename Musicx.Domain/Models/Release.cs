namespace Musicx.Domain.Models;

public sealed class Release : BaseModel
{
    public Album? Album { get; set; }
    public Label? Label { get; set; }
    
    public string? CatalogNumber { get; set; }
    public DateTime? ReleaseDate { get; set; }
}