using Musicx.Contracts.Enums;

namespace Musicx.Contracts.Dto.Responses;

public sealed class OutGenre : BaseOutputModel
{
    public IReadOnlyList<OutGenreClosure> Closures { get; set; } = [];
    
    public IReadOnlyList<OutGenreAlias>? Aliases { get; set; }
    public IReadOnlyList<OutGenreRelation>? Relations { get; set; }
    
    public bool IsVisible { get; set; }
    public string CanonicalName { get; set; } = null!;
    public bool IsTaggable { get; set; }
    public GenreType Type { get; set; }
    
    public string? Color { get; set; }
    public float? Confidence { get; set; } = 0.80f;
    public string? CountryOrigin { get; set; }
    public string? Description { get; set; }
    public DateTime? EraStart { get; set; }
    public DateTime? EraEnd { get; set; }
    public string? Metadata { get; set; }
    public string? ShortName { get; set; }
}