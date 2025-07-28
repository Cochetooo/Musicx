namespace Musicx.Contracts.Dto.Requests;

public sealed class InLabel : BaseInputModel
{
    // Required Relationships
    public IReadOnlyList<long> ReleaseIds { get; set; } = [];
    
    // Required Columns
    public string Name { get; set; } = null!;
    
    // Optional Columns
    public string? Description { get; set; }
}
    