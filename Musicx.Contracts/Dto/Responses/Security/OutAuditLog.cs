using Musicx.Contracts.Dto.Responses.User;

namespace Musicx.Contracts.Dto.Responses;

public sealed class OutAuditLog : BaseOutputModel
{
    public OutUser? User { get; set; }
    public long? UserId { get; set; }
    
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public long EntityId { get; set; }
    
    public object? OldData { get; set; }
    public object? NewData { get; set; }
}