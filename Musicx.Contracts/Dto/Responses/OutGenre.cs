namespace Musicx.Contracts.Dto.Responses;

public sealed class OutGenre
{
    public long Id { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public IReadOnlyList<OutGenre> Children { get; set; } = new List<OutGenre>();
    public IReadOnlyList<OutGenre> Parents { get; set; } = new List<OutGenre>();

    public string Name { get; set; } = null!;
    
    public string? Description { get; set; }
    public string? Color { get; set; }
}