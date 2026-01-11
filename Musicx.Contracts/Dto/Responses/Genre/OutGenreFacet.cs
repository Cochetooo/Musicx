using System.Text.Json.Nodes;

namespace Musicx.Contracts.Dto.Responses;

public sealed class OutGenreFacet : BaseOutputModel
{
    public OutFacet Facet { get; set; } = null!;
    public OutGenre Genre { get; set; } = null!;

    public float Confidence { get; set; } = 0.8f;
    public String? Metadata { get; set; }
    public string Value { get; set; } = null!;
}