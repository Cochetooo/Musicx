namespace Musicx.Contracts.Dto.Requests.Security;

public sealed class InAuditLog : BaseInputModel
{
    // Required Columns
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public long EntityId { get; set; }
    
    // Optional Columns
    public long? UserId { get; set; }
    public object? OldData { get; set; }
    public object? NewData { get; set; }
}