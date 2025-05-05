namespace Musicx.Contracts.Dto.Requests;

public sealed record InSong(
    // Primary Key
    long Id,
    
    // Required Relationships
    IReadOnlyList<long> PrimaryGenreIds,
    IReadOnlyList<long> InfluenceGenreIds,
    
    // Required Columns
    string Title,

    // Optional Relationships
    long? AlbumId,
    long? ArtistId,
    
    // Optional Columns
    int? DiscNumber,
    long? Duration,
    string? Lyrics,
    int? TrackNumber,
    ushort? BitRate,
    string? FilePath,
    string? Format,
    double? SampleRate,
    double? VolumeModifier);