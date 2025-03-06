using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Musicx.Models.Enums;
using MusicxApi.Models;

namespace Musicx.Entities;

[Table("Albums")]
public class AlbumEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public ulong Id { get; set; }

    // Relation avec l'Artiste principal
    public ulong? ArtistId { get; set; }
    [ForeignKey(nameof(ArtistId))]
    public virtual ArtistEntity? Artist { get; set; }

    // Relation avec le Label
    public ulong? LabelId { get; set; }
    [ForeignKey(nameof(LabelId))]
    public virtual LabelEntity? Label { get; set; }

    // Relations Many-to-Many avec Lazy Loading
    public virtual List<AlbumGenreEntity> Genres { get; set; } = new();
    public virtual List<AlbumInfluenceGenreEntity> InfluenceGenres { get; set; } = new();

    // Propriétés classiques
    public string? ArtworkUrl { get; set; }
    public string? CatalogNumber { get; set; }
    public uint? DiscTotal { get; set; }

    [Required]
    public required string Name { get; set; }

    public DateTime? ReleaseDate { get; set; }
    public ReleaseType? ReleaseType { get; set; }
    public uint? TrackTotal { get; set; }
}

[Table("AlbumGenres")]
public class AlbumGenreEntity
{
    [Key]
    public ulong AlbumId { get; set; }
    [ForeignKey(nameof(AlbumId))]
    public virtual AlbumEntity Album { get; set; } = null!;

    [Key]
    public ulong GenreId { get; set; }
    [ForeignKey(nameof(GenreId))]
    public virtual GenreEntity Genre { get; set; } = null!;
}

[Table("AlbumInfluenceGenres")]
public class AlbumInfluenceGenreEntity
{
    [Key]
    public ulong AlbumId { get; set; }
    [ForeignKey(nameof(AlbumId))]
    public virtual AlbumEntity Album { get; set; } = null!;

    [Key]
    public ulong GenreId { get; set; }
    [ForeignKey(nameof(GenreId))]
    public virtual GenreEntity Genre { get; set; } = null!;
}
