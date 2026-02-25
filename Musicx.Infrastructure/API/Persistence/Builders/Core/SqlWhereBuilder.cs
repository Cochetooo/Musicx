using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders.Core;

internal sealed class SqlWhereBuilder
{
    public (string sql, List<NpgsqlParameter> parameters)
        Build(IDictionary<string, object?> conditions)
    {
        if (!conditions.Any())
            return (string.Empty, new List<NpgsqlParameter>());

        var clauses = new List<string>();
        var parameters = new List<NpgsqlParameter>();

        foreach (var condition in conditions)
        {
            var paramName = "@" + condition.Key;
            clauses.Add($"{condition.Key} = {paramName}");
            parameters.Add(SqlParameterFactory.Create(condition.Key, condition.Value));
        }

        return ($"WHERE {string.Join(" AND ", clauses)}", parameters);
    }
}