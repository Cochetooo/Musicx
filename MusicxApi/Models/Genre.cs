namespace MusicxApi.Models;

public class Genre
{
    public ulong Id { get; set; }

    public List<ulong> ParentIds { get; set; } = [];
    public List<ulong> ChildIds { get; set; } = [];
    
    public required string Name { get; set; }
}