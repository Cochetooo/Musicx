using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Domain.Models;

namespace Musicx.Application.Api.Interfaces.Specifications;

/// <summary>
/// Specify the relation to include when retrieving release from repository.
/// </summary>
/// <since>0.6.1</since>
public class ReleaseQuerySpecification : IQuerySpecification<Release>
{
    /// <summary>
    /// Include album of this release.
    /// </summary>
    /// <since>0.6.1</since>
    public bool IncludeAlbum { get; set; }
    
    /// <summary>
    /// Include the label that issued this release.
    /// </summary>
    /// <since>0.6.1</since>
    public bool IncludeLabel { get; set; }
}