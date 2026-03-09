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
    
    internal void AppendTextFilter(
        TextFilter? filter,
        string column,
        string parameterPrefix,
        ICollection<string> clauses,
        ICollection<NpgsqlParameter> parameters,
        TextSearchFilterOptions? search)
    {
        if (filter is null || !filter.IsSet)
        {
            return;
        }

        if (search is { Exact: false } && filter.Mode == TextMatchMode.Contains)
        {
            var parameterName = $"@{parameterPrefix}Sim";
            clauses.Add($"similarity({column}, {parameterName}) >= {search.Similarity.ToString(CultureInfo.InvariantCulture)}");
            parameters.Add(new NpgsqlParameter(parameterName, filter.Value!));
            return;
        }

        var (condition, value) = BuildTextCondition(column, filter, parameterPrefix);
        if (string.IsNullOrWhiteSpace(condition))
        {
            return;
        }

        clauses.Add(condition);
        parameters.Add(new NpgsqlParameter($"@{parameterPrefix}", value));
    }
    
    internal void AppendAnyTextFilter(
        TextFilter filter,
        IEnumerable<string> columns,
        string parameterPrefix,
        ICollection<string> clauses,
        ICollection<NpgsqlParameter> parameters,
        TextSearchFilterOptions? search)
    {
        if (!filter.IsSet)
        {
            return;
        }

        var columnConditions = new List<string>();
        foreach (var (column, index) in columns.Select((c, i) => (c, i)))
        {
            var scopedClauses = new List<string>();
            var scopedParameters = new List<NpgsqlParameter>();
            AppendTextFilter(filter, column, $"{parameterPrefix}{index}", scopedClauses, scopedParameters, search);
            if (scopedClauses.Count == 1)
            {
                columnConditions.Add(scopedClauses[0]);
                foreach (var parameter in scopedParameters)
                {
                    parameters.Add(parameter);
                }
            }
        }

        if (columnConditions.Count > 0)
        {
            clauses.Add($"({string.Join(" OR ", columnConditions)})");
        }
    }
}