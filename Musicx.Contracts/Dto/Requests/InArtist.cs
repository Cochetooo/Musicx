namespace Musicx.Contracts.Dto.Requests;

public sealed record InArtist(
    // Primary Key
    long Id,
    
    // Required Columns
    string Discriminator,
    string Name,
    
    // Optional Relationships
    IReadOnlyList<long>? MemberIds,
    IReadOnlyList<long>? BandIds,
    
    // Optional Columns
    string? ArtworkUrl,
    string? Country,
    DateTime? FormationDate,
    DateTime? SplitDate,
    string? FirstName,
    string? LastName,
    DateTime? BirthDate,
    DateTime? DeathDate);