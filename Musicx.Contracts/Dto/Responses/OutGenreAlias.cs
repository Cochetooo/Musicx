using System.Text.Json.Nodes;

namespace Musicx.Contracts.Dto.Responses;

public sealed class OutGenreAlias : BaseOutputModel
{
    public OutGenre Genre { get; set; } = null!;

    public string? Lang { get; set; }
    public String? Metadata { get; set; }
    public string Name { get; set; } = null!;
}