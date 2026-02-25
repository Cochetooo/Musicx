using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;

namespace Musicx.Infrastructure.API.Persistence.Builders.Core.Query;

internal sealed class OrderByBuilder<T>
    where T : BaseInputModel
{
    /// <summary>
    /// Builds an ORDER BY clause from an OrderSpecification.
    /// Returns empty string if no ordering is defined.
    /// </summary>
    public string Build(OrderSpecification<T>? orderSpec)
    {
        if (orderSpec is null)
            return string.Empty;

        orderSpec.Validate();

        var clauses = orderSpec.ToClauses();

        if (!clauses.Any())
            return string.Empty;

        var sqlClauses = clauses
            .Select(c => $"{c.Field} {c.Direction}");

        return $"ORDER BY {string.Join(", ", sqlClauses)}";
    }
}