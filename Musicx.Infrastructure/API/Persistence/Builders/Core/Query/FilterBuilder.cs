using System.Globalization;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders.Core.Query;

internal sealed class FilterBuilder
{
    public (string sql, NpgsqlParameter parameter)
        Build(IEnumerable<string> columns,
            string filter,
            bool exact = false,
            double similarity = 0.4)
    {
        var param = SqlParameterFactory.Create("filter", filter);

        string condition = exact
            ? string.Join(" OR ", columns.Select(c => $"{c} ILIKE @filter"))
            : string.Join(" OR ", columns.Select(c =>
                $"similarity({c}, @filter) > {similarity.ToString(CultureInfo.InvariantCulture)}"));

        return ($"({condition})", param);
    }
}