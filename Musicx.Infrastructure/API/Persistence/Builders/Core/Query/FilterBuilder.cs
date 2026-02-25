using System.Globalization;
using Musicx.Application.API.Persistence.Filtering;
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
    
    internal void Filter(
        ref string sql,
        IEnumerable<string> columns,
        string filter,
        List<NpgsqlParameter> parameters,
        bool? filterExact = null,
        double? filterSimilitude = null)
    {
        if (string.IsNullOrWhiteSpace(filter) || !columns.Any())
        {
            return;
        }

        var (condition, parameter) = Build(
            columns,
            filter,
            filterExact ?? false,
            filterSimilitude ?? 0.4);

        parameters.Add(parameter);
        sql += $" WHERE {condition}";
    }

    internal void Filter(
        ref string sql,
        string column,
        string filter,
        List<NpgsqlParameter> parameters,
        bool? filterExact = null,
        double? filterSimilitude = null)
        => Filter(ref sql, [column], filter, parameters, filterExact, filterSimilitude);

    internal (string sql, object value) BuildTextCondition(string column, TextFilter filter, string paramName)
    {
        if (filter.Value is null)
        {
            return (string.Empty, string.Empty);
        }

        return filter.Mode switch
        {
            TextMatchMode.Equals =>
                ($"{column} = @{paramName}", filter.Value),

            TextMatchMode.StartsWith =>
                ($"{column} LIKE @{paramName}", $"{filter.Value}%"),

            TextMatchMode.Contains =>
                ($"{column} LIKE @{paramName}", $"%{filter.Value}%"),

            TextMatchMode.EndsWith =>
                ($"{column} LIKE @{paramName}", $"%{filter.Value}"),

            _ => throw new NotSupportedException("Text Match Mode not supported.")
        };
    }
}