using Musicx.Application.Shared.Interfaces.Persistence.Filtering;
using Musicx.Contracts.Dto.Requests;

namespace Musicx.Application.Shared.Interfaces.Persistence;

/// <summary>
/// Define the filters for a find endpoint.
/// </summary>
/// <typeparam name="T">A base model type</typeparam>
/// <since>0.6.8</since>
public interface IFindQuery<T>
    where T : BaseInputModel
{
    public TextSearchFilterOptions? Search { get; set; }
    public TextFilter? RawSearch { get; set; }
}