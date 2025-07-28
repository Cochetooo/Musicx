namespace Musicx.Contracts.Dto.Responses;

public class BaseOutputModel
{
    public long Id { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}