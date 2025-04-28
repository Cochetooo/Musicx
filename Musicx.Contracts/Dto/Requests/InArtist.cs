namespace Musicx.Contracts.Dto.Requests;

public sealed class InArtist : BaseModel
{
    public string? ArtworkUrl { get; set; }
    public string? Country { get; set; }
    public string Discriminator { get; set; } = null!;
    public string Name { get; set; } = null!;
    
    public ICollection<long> MemberIds { get; set; } = new List<long>();
    public DateTime? FormationDate { get; set; }
    public DateTime? SplitDate { get; set; }
    
    public ICollection<long> BandIds { get; set; } = new List<long>();
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime? DeathDate { get; set; }
}