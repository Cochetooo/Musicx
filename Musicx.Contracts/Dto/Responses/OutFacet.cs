using Musicx.Contracts.Enums;

namespace Musicx.Contracts.Dto.Responses;

public sealed class OutFacet : BaseOutputModel
{
    public string? Description { get; set; }
    public string Name { get; set; } = null!;
    public FacetType Type { get; set; }
}