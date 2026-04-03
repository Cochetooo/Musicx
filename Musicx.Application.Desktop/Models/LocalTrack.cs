namespace Musicx.Application.Desktop.Models;

public sealed class LocalTrack
{
    public Guid Id { get; set; }
    public long? ApiSongId { get; set; }
    public long? ApiAlbumId { get; set; }
    public long? ApiArtistId { get; set; }
    
    public TimeSpan Duration { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
    public bool IsMatchedWithApi { get; set; }
    public bool IsOwnedByUser { get; set; } = true;
    
    public string? Album { get; set; }
    public string? Artist { get; set; }
    public string? Title { get; set; }
}