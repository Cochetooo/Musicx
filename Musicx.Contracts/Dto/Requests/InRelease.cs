namespace Musicx.Contracts.Dto.Requests;

public sealed class InRelease(
    // Primary Key
    long Id,
    
    // Required Relationships
    long AlbumId,
    
    // Required Columns
    string CatalogNumber,

    // Optional Relationships
    long? LabelId,
    
    // Optional Columns
    DateTime? ReleaseDate);