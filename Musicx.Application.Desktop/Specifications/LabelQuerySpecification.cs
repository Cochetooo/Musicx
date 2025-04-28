using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Domain.Models;

namespace Musicx.Application.Desktop.Specifications;

/// <summary>
/// Specify the relation to include when retrieving label from repository.
/// </summary>
/// <since>0.6.1</since>
public class LabelQuerySpecification : IQuerySpecification<Label>
{
    /// <summary>
    /// Include the releases issued by this label.
    /// </summary>
    /// <since>0.6.1</since>
    public bool IncludeReleases { get; set; }
}