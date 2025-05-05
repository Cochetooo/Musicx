namespace Musicx.Contracts.Dto.Requests;

public sealed record InAlbum(
    // Primary Key
    long Id,
    
    // Required Relationships
    IReadOnlyList<long> ReleaseIds,
    IReadOnlyList<long> PrimaryGenreIds,
    IReadOnlyList<long> InfluenceGenreIds,
    
    // Required Columns
    string Name,

    // Optional Relationships
    long? ArtistId,
    
    // Optional Columns
    string? ArtworkUrl,
    int? DiscTotal,
    DateTime? ReleaseDate,
    string? ReleaseType,
    int? TrackTotal);