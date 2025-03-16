using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Musicx.Data.Entities;

[Table("Artists")]
public abstract class ArtistEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public ulong Id { get; set; }
    
    public string? ArtworkUrl { get; set; }
    public string? Country { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;
    
    public string Discriminator { get; set; } = string.Empty;
}

public class BandArtistEntity : ArtistEntity
{
    public virtual List<PersonBandEntity> Members { get; set; } = [];

    public DateTime? FormationDate { get; set; }
    public DateTime? SplitDate { get; set; }
}

public class PersonArtistEntity : ArtistEntity
{
    public virtual List<PersonBandEntity> Bands { get; set; } = [];

    [Required]
    public string FirstName { get; set; } = string.Empty;

    public DateTime? BirthDate { get; set; }
    public DateTime? DeathDate { get; set; }
}

[Table("PersonBandEntity")]
[PrimaryKey(nameof(PersonId), nameof(BandId))]
public class PersonBandEntity
{
    public ulong PersonId { get; set; }
    public ulong BandId { get; set; }
}