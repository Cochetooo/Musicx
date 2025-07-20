using Musicx.Contracts.Dto.Enums;

namespace Musicx.Contracts.Dto.Responses;

public sealed class OutArtist
{
    public long Id { get; set; }
        
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public string? ArtworkUrl { get; set; }
    public string? Country { get; set; }
    public ArtistDiscriminator Discriminator { get; set; }
    public string Name { get; set; } = null!;

    public IReadOnlyList<OutArtist> Members { get; set; } = [];
    public DateTime? FormationDate { get; set; }
    public DateTime? SplitDate { get; set; }

    public IReadOnlyList<OutArtist> Bands { get; set; } = [];
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime? DeathDate { get; set; }
}