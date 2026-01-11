using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Label;

namespace Musicx.Infrastructure.API.Persistence.Specifications.Label;

/// <summary>
/// Specify the relation to include when retrieving label from repository.
/// </summary>
/// <since>0.6.1</since>
public class LabelJoinSpecification : IJoinSpecification<InLabel>
{
    /// <summary>
    /// Include the releases issued by this label.
    /// </summary>
    /// <since>0.6.1</since>
    public bool IncludeReleases { get; set; }
}