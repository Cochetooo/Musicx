namespace Musicx.Domain.Models;

public sealed class Genre : BaseModel
{
    public IReadOnlyList<Genre> Parents { get; set; } = new List<Genre>();
    public IReadOnlyList<Genre> Children { get; set; } = new List<Genre>();
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Color { get; set; }
}