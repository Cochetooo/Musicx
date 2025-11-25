using System.Text.Json.Serialization;
using Musicx.Contracts.Enums;

namespace Musicx.Contracts.Dto.Responses;

public sealed class OutGenre : BaseOutputModel
{
    public IReadOnlyList<OutGenreClosure> Closures { get; set; } = [];
    
    public IReadOnlyList<OutGenreAlias>? Aliases { get; set; }
    public IReadOnlyList<OutGenreRelation>? Relations { get; set; }
    
    public bool IsVisible { get; set; }
    public string Name { get; set; } = null!;
    public GenreType Type { get; set; }
    
    public string? Description { get; set; }
    public string? Color { get; set; }
}