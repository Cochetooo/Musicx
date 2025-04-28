namespace Musicx.Contracts.Dto.Responses;

public sealed class OutGenre : BaseModel
{
    public ICollection<OutGenre> Children { get; set; } = new List<OutGenre>();
    public ICollection<OutGenre> Parents { get; set; } = new List<OutGenre>();
    
    public string Name { get; set; } = null!;
}