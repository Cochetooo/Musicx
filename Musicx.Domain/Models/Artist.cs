namespace Musicx.Domain.Models;

public class Artist : BaseModel
{
    public string? ArtworkUrl { get; set; }
    public string? Country { get; set; }
    public string Name { get; set; } = null!;
}