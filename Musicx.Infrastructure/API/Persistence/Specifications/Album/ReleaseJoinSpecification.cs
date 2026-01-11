using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Album;

namespace Musicx.Infrastructure.API.Persistence.Specifications.Album;

/// <summary>
/// Specify the relation to include when retrieving release from repository.
/// </summary>
/// <since>0.6.1</since>
public class ReleaseJoinSpecification : IJoinSpecification<InRelease>
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