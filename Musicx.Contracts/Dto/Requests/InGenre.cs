namespace Musicx.Contracts.Dto.Requests;

public sealed class InGenre(
    // Primary Key
    long Id,
    
    // Required Relationships
    IReadOnlyList<long> ChildIds,
    IReadOnlyList<long> ParentIds,
    
    // Required Columns
    string Name);