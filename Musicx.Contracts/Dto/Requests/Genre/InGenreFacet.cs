namespace Musicx.Contracts.Dto.Requests.Genre;

public sealed class InGenreFacet : BaseInputModel
{
    // Required Relationships
    public long FacetId { get; set; }
    public long GenreId { get; set; }
    
    // Required Columns
    public float Confidence { get; set; } = 0.8f;
    public string Value { get; set; } = null!;
    
    // Optional Columns
    public string? Metadata { get; set; }
}