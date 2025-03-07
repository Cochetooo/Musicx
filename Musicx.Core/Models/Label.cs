namespace Musicx.Core.Models;

public class Label
{
    public ulong Id { get; set; }

    public List<ulong> AlbumIds { get; set; } = [];
    
    public string? Description { get; set; }
    public string Name { get; set; } = string.Empty;
}