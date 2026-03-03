using Musicx.Contracts.Dto.Responses.User;

namespace Musicx.Contracts.Dto.Responses;

public sealed class OutEventUser : BaseOutputModel
{
    public OutEvent Event { get; set; } = null!;
    public OutUser User { get; set; } = null!;
    
    public string? Comment { get; set; }
    public bool IsGoing { get; set; }
}