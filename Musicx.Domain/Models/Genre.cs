namespace Musicx.Domain.Models;

public sealed class Genre : BaseModel
{
    public ICollection<Genre> Children { get; set; } = new List<Genre>();
    public ICollection<Genre> Parents { get; set; } = new List<Genre>();
    public string Name { get; set; } = null!;
}