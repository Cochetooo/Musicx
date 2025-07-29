namespace Musicx.Contracts.Dto.Responses;

public abstract class BaseOutputModel
{
    public long Id { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}