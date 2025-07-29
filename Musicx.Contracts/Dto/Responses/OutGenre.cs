using System.Text.Json.Serialization;

namespace Musicx.Contracts.Dto.Responses;

public sealed class OutGenre : BaseOutputModel
{
    public IReadOnlyList<OutGenre>? Children { get; set; }
    public IReadOnlyList<OutGenre>? Parents { get; set; }
    
    public string Name { get; set; } = null!;
    
    public string? Description { get; set; }
    public string? Color { get; set; }
}