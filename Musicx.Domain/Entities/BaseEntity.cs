namespace Musicx.Domain.Entities;

/// <summary>
/// Generic model of an entity.
/// </summary>
/// <since>0.6.1</since>
public abstract class BaseEntity
{
    /// <summary>
    /// Unique database identifier
    /// </summary>
    /// <since>0.3.0</since>
    public long Id { get; set; }
}