namespace Musicx.Domain.Entities;

/// <summary>
/// Entity representing a record label.
/// </summary>
/// <since>0.3.0</since>
public sealed class Label : BaseEntity
{
    /// <summary>
    /// List of releases assigned to this label.
    /// </summary>
    /// <since>0.6.1</since>
    public ICollection<Release>? Releases { get; set; }
    
    /// <summary>
    /// Description and history of this label.
    /// </summary>
    /// <since>0.6.0</since>
    public string? Description { get; set; }
    
    /// <summary>
    /// Name of this record label.
    /// </summary>
    /// <since>0.3.0</since>
    public string Name { get; set; } = string.Empty;
}