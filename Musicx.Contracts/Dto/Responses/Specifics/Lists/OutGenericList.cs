namespace Musicx.Contracts.Dto.Responses.Specifics.Lists;

public sealed class OutGenericList<T> 
    where T : BaseOutputModel
{
    public IReadOnlyList<T> Items { get; set; } = [];
    public long Total { get; set; }
}