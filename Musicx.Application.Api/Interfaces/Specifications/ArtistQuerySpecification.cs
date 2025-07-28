using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;


namespace Musicx.Application.Api.Interfaces.Specifications;

/// <summary>
/// Specify the relation to include when retrieving artist from repository.
/// </summary>
/// <since>0.6.1</since>
public class ArtistQuerySpecification : IQuerySpecification<InArtist>
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
}