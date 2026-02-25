using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders.Core.Commands;

internal sealed class UpdateBuilder
{
    /// <summary>
    /// Builds a parameterized UPDATE statement.
    /// 
    /// Supports composite keys via multiple WHERE columns.
    /// Ignores "Id" property in update set.
    /// </summary>
    /// <param name="table">Table to update.</param>
    /// <param name="properties">Columns and values to update.</param>
    /// <param name="where">Columns and values matching</param>
    /// <returns>SqlStatement ready for execution.</returns>
    public SqlStatement Build(
        string table,
        IDictionary<string, object?> properties,
        IDictionary<string, object?> where)
    {
        var setters = new List<string>();
        var parameters = new List<NpgsqlParameter>();

        foreach (var prop in properties.Where(p => p.Key != "Id"))
        {
            setters.Add($"{prop.Key} = @{prop.Key}");
            parameters.Add(SqlParameterFactory.Create(prop.Key, prop.Value));
        }

        var whereBuilder = new SqlWhereBuilder();
        var (whereSql, whereParams) = whereBuilder.Build(where);

        parameters.AddRange(whereParams);

        var sql = $"UPDATE {table} SET {string.Join(", ", setters)} {whereSql}";

        return new SqlStatement(sql, parameters);
    }
}