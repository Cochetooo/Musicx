namespace Musicx.Contracts.Dto.Responses;

public sealed class OutLabel
{
    public long Id { get; set; }
    
    public IReadOnlyList<OutRelease> Releases { get; set; } = new List<OutRelease>();
    
    public string? Description { get; set; }
    public string Name { get; set; } = null!;
}