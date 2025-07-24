namespace Musicx.Domain.Models;

public sealed class Genre : BaseModel
{
    public IReadOnlyList<long> ParentIds { get; set; } = new List<long>();
    public IReadOnlyList<long> ChildIds { get; set; } = new List<long>();
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Color { get; set; }
}