namespace Musicx.Contracts.Dto.Requests;

public sealed class InGenre : BaseInputModel
{
    // Required Relationships
    public IReadOnlyList<long> ChildIds { get; set; } = [];
    public IReadOnlyList<long> ParentIds { get; set; } = [];
    
    // Required Columns
    public string Name { get; set; } = null!;
    
    // Optional Columns
    public string? Description { get; set; }
    public string? Color { get; set; }
}