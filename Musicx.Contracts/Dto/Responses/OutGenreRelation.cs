using System.Text.Json.Nodes;
using Musicx.Contracts.Enums;

namespace Musicx.Contracts.Dto.Responses;

public sealed class OutGenreRelation : BaseOutputModel
{
    public OutGenre FromGenre { get; set; } = null!;
    public OutGenre ToGenre { get; set; } = null!;
    
    public JsonObject? Metadata { get; set; }
    public GenreRelationType Type { get; set; }
    public float Weight { get; set; } = 1.0f;
}