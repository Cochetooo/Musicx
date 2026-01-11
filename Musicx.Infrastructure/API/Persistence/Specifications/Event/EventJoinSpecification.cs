using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Event;

namespace Musicx.Infrastructure.API.Persistence.Specifications.Event;

/// <summary>
/// Specify the relation to include when retrieving event from repository.
/// </summary>
/// <since>0.6.9</since>
public class EventJoinSpecification : IJoinSpecification<InEvent>
{
    /// <summary>
    /// Include artists.
    /// </summary>
    /// <since>0.6.9</since>
    public bool IncludeArtists { get; set; }
    
    /// <summary>
    /// Include users.
    /// </summary>
    /// <since>0.6.9</since>
    public bool IncludeUsers { get; set; }
}