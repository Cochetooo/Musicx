namespace Musicx.Contracts.Dto.Requests.Event;

public sealed class InEventUser : BaseInputModel
{
    public long EventId { get; set; }
    public long UserId { get; set; }
    
    public bool IsGoing { get; set; }
    
    public string? Comment { get; set; }
}