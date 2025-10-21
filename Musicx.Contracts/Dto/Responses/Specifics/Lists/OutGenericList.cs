namespace Musicx.Contracts.Dto.Responses.Specifics.Lists;

public class OutGenericList<T> 
    where T : BaseOutputModel
{
    public List<T> Items { get; set; } = [];
    public long Total { get; set; }
}