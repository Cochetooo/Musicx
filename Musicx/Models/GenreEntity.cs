namespace Musicx.Models;

public class GenreEntity
{
    public long Id { get; set; }

    public List<long> ParentIds { get; } = new();
    
    public required string Name { get; set; }
}