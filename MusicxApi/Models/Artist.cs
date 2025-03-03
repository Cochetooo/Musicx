namespace MusicxApi.Models;

public class Artist
{
    public ulong Id { get; set; }
    
    public string? ArtworkUrl { get; set; }
    public string? Country { get; set; }
    public required string Name { get; set; }
}