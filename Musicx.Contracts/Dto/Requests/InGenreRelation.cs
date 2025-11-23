using System.Text.Json.Nodes;
using Musicx.Contracts.Enums;

namespace Musicx.Contracts.Dto.Requests;

public sealed class InGenreRelation : BaseInputModel
{
    // Required Relationships
    public long FromGenreId { get; set; }
    public long ToGenreId { get; set; }
    
    // Required Columns
    public GenreRelationType Type { get; set; }
    public float Weight { get; set; } = 1.0f;
    
    // Optional Columns
    public JsonObject? Metadata { get; set; }
}