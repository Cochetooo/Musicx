using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MusicxApi.Models;

namespace Musicx.Entities;

[Table("Artists")]
public abstract class ArtistEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public ulong Id { get; set; }
    
    public string? ArtworkUrl { get; set; }
    public string? Country { get; set; }

    [Required]
    public required string Name { get; set; }
    
    public string Discriminator { get; set; } = string.Empty;
}

public class BandArtistEntity : ArtistEntity
{
    public List<ulong> MemberIds { get; set; } = [];

    public DateTime? FormationDate { get; set; }
    public DateTime? SplitDate { get; set; }
}

public class PersonArtistEntity : ArtistEntity
{
    public List<ulong> BandIds { get; set; } = [];

    [Required]
    public required string FirstName { get; set; }

    public DateTime? BirthDate { get; set; }
    public DateTime? DeathDate { get; set; }
}