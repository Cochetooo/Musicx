namespace Musicx.Models;

public class LabelEntity
{
    public long Id { get; set; }
    
    public string? Description { get; set; }
    public required string Name { get; set; }
}