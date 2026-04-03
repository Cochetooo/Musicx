using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Application.Shared.Interfaces.Persistence.Filtering;
using Musicx.Contracts.Dto.Requests;

namespace Musicx.Application.Shared.Models.Queries;

public sealed record FindQuery<T> : IFindQuery<T>
    where T : BaseInputModel
{
    public TextSearchFilterOptions? Search { get; set; }
    public TextFilter? RawSearch { get; set; }

    public static FindQuery<T>? Create(string? filter = null, bool exact = false, double similarity = 0.4)
    {
        if (string.IsNullOrWhiteSpace(filter))
        {
            return null;
        }

        return new FindQuery<T>
        {
            Search = new TextSearchFilterOptions { Exact = exact, Similarity = similarity },
            RawSearch = new TextFilter(filter)
        };
    }
}