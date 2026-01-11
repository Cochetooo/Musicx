namespace Musicx.Contracts.Dto.Requests.Genre;

public sealed class InGenreAlias : BaseInputModel
{
    // Required Relationships
    public long GenreId { get; set; }
    
    // Required Columns
    public string Name { get; set; } = null!;
    
    // Optional Columns
    public string? Lang { get; set; }
    public string? Metadata { get; set; }
}