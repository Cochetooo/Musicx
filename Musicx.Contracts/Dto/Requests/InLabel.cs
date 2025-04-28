namespace Musicx.Contracts.Dto.Requests;

public sealed class InLabel : BaseModel
{
    public ICollection<long> ReleaseIds { get; set; } = new List<long>();
    
    public string? Description { get; set; }
    public string Name { get; set; } = null!;
}