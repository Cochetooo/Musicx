using System.Text.Json.Nodes;
using Musicx.Contracts.Enums;

namespace Musicx.Contracts.Dto.Requests;

public sealed class InGenre : BaseInputModel
{
    // Required Columns
    public string CanonicalName { get; set; } = null!;
    public bool IsVisible { get; set; }
    public bool IsTaggable { get; set; }
    public GenreType Type { get; set; }
    
    // Optional Columns
    public string? Color { get; set; }
    public float? Confidence { get; set; } = 0.8f;
    public string? CountryOrigin { get; set; }
    public string? Description { get; set; }
    public DateTime? EraStart { get; set; }
    public DateTime? EraEnd { get; set; }
    public JsonObject? Metadata { get; set; }
    public string? ShortName { get; set; }
}