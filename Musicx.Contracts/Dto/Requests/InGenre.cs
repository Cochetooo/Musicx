namespace Musicx.Contracts.Dto.Requests;

public sealed class InGenre : BaseModel
{
    public ICollection<long> ChildIds { get; set; } = new List<long>();
    public ICollection<long> ParentIds { get; set; } = new List<long>();
    public string Name { get; set; } = null!;
}