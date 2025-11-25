using System.Text.Json.Nodes;

namespace Musicx.Contracts.Dto.Responses;

public sealed class OutGenreAlias : BaseOutputModel
{
    public OutGenre Genre { get; set; } = null!;

    public string? Lang { get; set; }
    public JsonObject? Metadata { get; set; }
    public string Name { get; set; } = null!;
}