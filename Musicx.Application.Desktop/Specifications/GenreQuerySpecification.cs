using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Domain.Models;

namespace Musicx.Application.Desktop.Specifications;

/// <summary>
/// Specify the relation to include when retrieving genre from repository.
/// </summary>
/// <since>0.6.1</since>
public record GenreQuerySpecification : IQuerySpecification<Genre>
{
    /// <summary>
    /// Include children genres of this genre.
    /// </summary>
    /// <since>0.6.1</since>
    public bool IncludeChildren { get; set; }
    
    /// <summary>
    /// Include parents genres of this genre.
    /// </summary>
    /// <since>0.6.1</since>
    public bool IncludeParents { get; set; }
}