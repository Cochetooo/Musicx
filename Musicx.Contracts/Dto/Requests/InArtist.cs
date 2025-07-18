using Musicx.Contracts.Dto.Enums;

namespace Musicx.Contracts.Dto.Requests;

public sealed class InArtist
{
    // Primary Key
    public long Id { get; set; }
    
    // Required Columns
    public ArtistDiscriminator Discriminator { get; set; }
    public string Name { get; set; } = string.Empty;
    
    // Optional Relationships
    public IReadOnlyList<long>? MemberIds { get; set; } = [];
    public IReadOnlyList<long>? BandIds { get; set; } = [];
    
    // Optional Columns
    public string? ArtworkUrl { get; set; }
    public string? Country { get; set; }
    public DateTime? FormationDate { get; set; }
    public DateTime? SplitDate { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime? DeathDate { get; set; }
}