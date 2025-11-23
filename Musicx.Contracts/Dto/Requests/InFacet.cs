using Musicx.Contracts.Enums;

namespace Musicx.Contracts.Dto.Requests;

public sealed class InFacet : BaseInputModel
{
    // Required Columns
    public string Name { get; set; } = null!;
    public FacetType Type { get; set; }
    
    // Optional Columns
    public string? Description { get; set; }
}