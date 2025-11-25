using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;


namespace Musicx.Application.Api.Interfaces.Specifications;

/// <summary>
/// Specify the relation to include when retrieving genre from repository.
/// </summary>
/// <since>0.6.1</since>
public record GenreQuerySpecification : IQuerySpecification<InGenre>
{
    /// <summary>
    /// Include aliases for this genre.
    /// </summary>
    /// <since>0.6.9</since>
    public bool IncludeAliases { get; set; }
    
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
    
    /// <summary>
    /// Include detailed relations for this genre.
    /// </summary>
    /// <since>0.6.9</since>
    public bool IncludeRelations { get; set; }
}