namespace Musicx.Contracts.Dto.Requests.Security;

public sealed class InRole : BaseInputModel
{
    // Required Columns
    public string Name { get; set; } = string.Empty;
    
    // Optional Relationships
    public IReadOnlyList<long>? PermissionIds { get; set; }
}