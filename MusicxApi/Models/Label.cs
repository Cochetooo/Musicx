namespace MusicxApi.Models;

public class Label
{
    public ulong Id { get; set; }

    public List<ulong> AlbumIds { get; set; } = [];
    
    public string? Description { get; set; }
    public required string Name { get; set; }
}