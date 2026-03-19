using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Artist;

namespace Musicx.Infrastructure.API.Persistence.Specifications.Artist;

/// <summary>
/// Specify the relation to include when retrieving artist from repository.
/// </summary>
/// <since>0.6.1</since>
public class ArtistJoinSpecification : IJoinSpecification<InArtist>
{
    /// <summary>
    /// Include members if this artist is a band.
    /// </summary>
    /// <since>0.6.1</since>
    public bool IncludeMembers { get; set; }
    
    /// <summary>
    /// Include bands if this artist is a person.
    /// </summary>
    /// <since>0.6.1</since>
    public bool IncludeBands { get; set; }
    
    /// <summary>
    /// Include rating stats derived from album ratings.
    /// </summary>
    /// <since>0.7.3</since>
    public bool IncludeStats { get; set; }
}