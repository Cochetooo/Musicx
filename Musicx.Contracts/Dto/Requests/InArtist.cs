namespace Musicx.Contracts.Dto.Requests;

public sealed class InArtist
{
    // Primary Key
    public long Id { get; set; }
    
    // Required Columns
    public string Discriminator { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    
    // Optional Relationships
    public IReadOnlyList<long>? MemberIds { get; set; } = [];
    public IReadOnlyList<long>? BandIds { get; set; } = [];
    
    // Optional Columns
    public string? ArtworkUrl { get; set; } = string.Empty;
    public string? Country { get; set; } = string.Empty;
    public DateTime? FormationDate { get; set; } = System.DateTime.MinValue;
    public DateTime? SplitDate { get; set; } = System.DateTime.MinValue;
    public string? FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; } = System.DateTime.MinValue;
    public DateTime? DeathDate { get; set; } = System.DateTime.MinValue;
}