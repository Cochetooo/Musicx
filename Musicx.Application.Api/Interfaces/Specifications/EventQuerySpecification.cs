using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;

namespace Musicx.Application.Api.Interfaces.Specifications;

/// <summary>
/// Specify the relation to include when retrieving event from repository.
/// </summary>
/// <since>0.6.9</since>
public class EventQuerySpecification : IQuerySpecification<InEvent>
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