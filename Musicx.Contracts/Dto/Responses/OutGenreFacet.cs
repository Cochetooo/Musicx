using System.Text.Json.Nodes;

namespace Musicx.Contracts.Dto.Responses;

public sealed class OutGenreFacet : BaseOutputModel
{
    public OutFacet Facet { get; set; }
    public OutGenre Genre { get; set; }

    public float Confidence { get; set; } = 0.8f;
    public JsonObject? Metadata { get; set; }
    public string Value { get; set; } = null!;
}