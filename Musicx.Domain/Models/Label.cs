namespace Musicx.Domain.Models;

public sealed class Label : BaseModel
{
    public ICollection<Release> Releases { get; set; } = new List<Release>();
    
    public string? Description { get; set; }
    public string Name { get; set; } = null!;
}