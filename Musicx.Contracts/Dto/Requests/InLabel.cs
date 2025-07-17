namespace Musicx.Contracts.Dto.Requests;

public sealed class InLabel(
    // Primary Key
    long Id,
    
    // Required Relationships
    IReadOnlyList<long> ReleaseIds,
    
    // Required Columns
    string Name,
    
    // Optional Columns
    string? Description);