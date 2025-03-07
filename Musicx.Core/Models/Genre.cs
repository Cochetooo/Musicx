namespace Musicx.Core.Models;

public class Genre
{
    public ulong Id { get; set; }

    public List<ulong> ParentIds { get; set; } = [];
    public List<ulong> ChildIds { get; set; } = [];
    
    public string Name { get; set; } = string.Empty;
}