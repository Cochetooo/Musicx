using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Musicx.Core.Models.Enums;

namespace Musicx.Data.Entities;

[Table("Albums")]
public class AlbumEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public ulong Id { get; set; }

    // Relation avec l'Artiste principal
    public ulong? ArtistId { get; set; }

    // Relation avec le Label
    public ulong? LabelId { get; set; }

    // Relations Many-to-Many avec Lazy Loading
    public virtual List<AlbumGenreEntity> Genres { get; set; } = new();
    public virtual List<AlbumInfluenceGenreEntity> InfluenceGenres { get; set; } = new();

    // Propriétés classiques
    public string? ArtworkUrl { get; set; }
    public string? CatalogNumber { get; set; }
    public uint? DiscTotal { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public DateTime? ReleaseDate { get; set; }
    public ReleaseType? ReleaseType { get; set; }
    public uint? TrackTotal { get; set; }
}

[Table("AlbumGenres")]
[PrimaryKey(nameof(AlbumId), nameof(GenreId))]
public class AlbumGenreEntity
{
    public ulong AlbumId { get; set; }
    public ulong GenreId { get; set; }
}

[Table("AlbumInfluenceGenres")]
[PrimaryKey(nameof(AlbumId), nameof(GenreId))]
public class AlbumInfluenceGenreEntity
{
    public ulong AlbumId { get; set; }
    public ulong GenreId { get; set; }
}
