using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Musicx.Core.Models.Enums;

namespace Musicx.Data.Entities;

[Table("Songs")]
public class SongEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public ulong Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.MinValue;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Relation avec l'album
    public ulong? AlbumId { get; set; }
    [ForeignKey(nameof(AlbumId))]
    public virtual AlbumEntity? Album { get; set; }

    // Relation avec l'artiste principal
    public ulong? ArtistId { get; set; }
    [ForeignKey(nameof(ArtistId))]
    public virtual ArtistEntity? Artist { get; set; }

    // Relations Many-to-Many pour les genres et influences
    public virtual List<SongGenreEntity> Genres { get; set; } = new();
    public virtual List<SongInfluenceGenreEntity> InfluenceGenres { get; set; } = new();

    // Propriétés audio
    [Required]
    public AudioFormatType AudioFormat { get; set; } = AudioFormatType.Unknown;
    
    [Required]
    public int BitRate { get; set; }
    
    public uint? DiscNumber { get; set; }
    
    [Required]
    public long Duration { get; set; }
    
    [Required]
    public string Filepath { get; set; } = string.Empty;
    
    public string? GeneratedGenreName { get; set; }
    public string? Lyrics { get; set; }
    
    [Required]
    public int SampleRate { get; set; }
    
    public string Title { get; set; } = string.Empty;
    public uint? TrackNumber { get; set; }
}

[Table("SongGenres")]
[PrimaryKey("SongId", "GenreId")]
public class SongGenreEntity
{
    public ulong SongId { get; set; }
    [ForeignKey(nameof(SongId))]
    public virtual SongEntity Song { get; set; } = null!;
    
    public ulong GenreId { get; set; }
    [ForeignKey(nameof(GenreId))]
    public virtual GenreEntity Genre { get; set; } = null!;
}

[Table("SongInfluenceGenres")]
[PrimaryKey("SongId", "GenreId")]
public class SongInfluenceGenreEntity
{
    public ulong SongId { get; set; }
    [ForeignKey(nameof(SongId))]
    public virtual SongEntity Song { get; set; } = null!;
    
    public ulong GenreId { get; set; }
    [ForeignKey(nameof(GenreId))]
    public virtual GenreEntity Genre { get; set; } = null!;
}
