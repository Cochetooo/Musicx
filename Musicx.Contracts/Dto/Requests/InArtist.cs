using Musicx.Contracts.Enums;

namespace Musicx.Contracts.Dto.Requests;

public sealed class InArtist : BaseInputModel
{
    // Required Columns
    public ArtistDiscriminator Discriminator { get; set; }
    public bool IsVisible { get; set; }
    public string Name { get; set; } = string.Empty;
    
    // Optional Relationships
    public IReadOnlyList<long>? MemberIds { get; set; } = [];
    public IReadOnlyList<long>? BandIds { get; set; } = [];
    
    // Optional Columns
    public string? Alias { get; set; }
    public string? ArtworkUrl { get; set; }
    public string? CurrentCountry { get; set; }
    public string? CurrentRegion { get; set; }
    public string? CurrentTown { get; set; }
    public string? Description { get; set; }
    public string? OriginCountry { get; set; }
    public string? OriginRegion { get; set; }
    public string? OriginTown { get; set; }
    
    public DateTime? FormationDate { get; set; }
    public DateTime? SplitDate { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime? DeathDate { get; set; }
}