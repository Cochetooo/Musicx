namespace Musicx.Contracts.Dto.Responses;

public sealed class OutRole : BaseOutputModel
{
    public ICollection<OutPermission>? Permissions { get; set; }
    
    public string Name { get; set; } = string.Empty;
}