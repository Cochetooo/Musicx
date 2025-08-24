using System.Text.Json.Serialization;
using Musicx.Contracts.Enums;

namespace Musicx.Contracts.Dto.Responses;

public sealed class OutGenre : BaseOutputModel
{
    public IReadOnlyList<OutGenre>? Children { get; set; }
    public IReadOnlyList<OutGenre>? Parents { get; set; }
    
    public string Name { get; set; } = null!;
    public GenreType Type { get; set; }
    
    public string? Description { get; set; }
    public string? Color { get; set; }
}