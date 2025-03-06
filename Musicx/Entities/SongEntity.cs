using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Musicx.Models.Enums;

namespace Musicx.Entities;

[Table("Songs")]
public class SongEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public ulong Id { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

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
    public required AudioFormatType AudioFormat { get; set; }
    
    [Required]
    public required int BitRate { get; set; }
    
    public uint? DiscNumber { get; set; }
    
    [Required]
    public required long Duration { get; set; }
    
    [Required]
    public required string Filepath { get; set; }
    
    public string? GeneratedGenreName { get; set; }
    public string? Lyrics { get; set; }
    
    [Required]
    public required int SampleRate { get; set; }
    
    public string Title { get; set; } = "Untitled";
    public uint? TrackNumber { get; set; }
}

[Table("SongGenres")]
public class SongGenreEntity
{
    [Key]
    public ulong SongId { get; set; }
    [ForeignKey(nameof(SongId))]
    public virtual SongEntity Song { get; set; } = null!;

    [Key]
    public ulong GenreId { get; set; }
    [ForeignKey(nameof(GenreId))]
    public virtual GenreEntity Genre { get; set; } = null!;
}

[Table("SongInfluenceGenres")]
public class SongInfluenceGenreEntity
{
    [Key]
    public ulong SongId { get; set; }
    [ForeignKey(nameof(SongId))]
    public virtual SongEntity Song { get; set; } = null!;

    [Key]
    public ulong GenreId { get; set; }
    [ForeignKey(nameof(GenreId))]
    public virtual GenreEntity Genre { get; set; } = null!;
}
