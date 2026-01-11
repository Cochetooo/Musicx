namespace Musicx.Contracts.Dto.Requests.Label;

public sealed class InLabel : BaseInputModel
{
    // Required Relationships
    public IReadOnlyList<long> ReleaseIds { get; set; } = [];
    
    // Required Columns
    public bool IsVisible { get; set; }
    public string Name { get; set; } = null!;
    
    // Optional Columns
    public string? Description { get; set; }
}
    