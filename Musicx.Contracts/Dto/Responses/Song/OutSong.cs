using Musicx.Contracts.Dto.Responses.Artist;
using Musicx.Contracts.Dto.Responses.Genre;
using Musicx.Contracts.Enums;

namespace Musicx.Contracts.Dto.Responses;

public sealed class OutSong : BaseOutputModel
{
    public OutAlbum? Album { get; set; }
    public OutArtist? Artist { get; set; }
    public IReadOnlyList<OutGenre>? PrimaryGenres { get; set; } = new List<OutGenre>();
    public IReadOnlyList<OutGenre>? InfluenceGenres { get; set; } = new List<OutGenre>();
    
    public long? AlbumId { get; set; }
    public long? ArtistId { get; set; }
    
    public int? DiscNumber { get; set; }
    public long? Duration { get; set; }
    public bool IsVisible { get; set; }
    public string? Lyrics { get; set; }
    public string Title { get; set; } = null!;
    public int? TrackNumber { get; set; }
    public SongType Type { get; set; }
}