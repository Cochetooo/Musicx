namespace Musicx.Contracts.Dto.Responses;

public sealed class OutArtist : BaseModel
{
    public string? ArtworkUrl { get; set; }
    public string? Country { get; set; }
    public string Discriminator { get; set; } = null!;
    public string Name { get; set; } = null!;
    
    public ICollection<OutArtist> Members { get; set; } = new List<OutArtist>();
    public DateTime? FormationDate { get; set; }
    public DateTime? SplitDate { get; set; }

    public ICollection<OutArtist> Bands { get; set; } = new List<OutArtist>();
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime? DeathDate { get; set; }
}