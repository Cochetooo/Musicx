namespace Musicx.Core.Models;

public abstract class Artist
{
    public ulong Id { get; set; }
    
    public string? ArtworkUrl { get; set; }
    public string? Country { get; set; }
    public string Name { get; set; } = string.Empty;
}