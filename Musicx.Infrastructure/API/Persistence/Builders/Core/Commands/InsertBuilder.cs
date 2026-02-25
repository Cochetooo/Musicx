using Musicx.Infrastructure.API.Persistence.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders.Core.Commands;

internal class InsertBuilder
{
    /// <summary>
    /// Builds a parameterized INSERT statement.
    /// 
    /// Handles:
    /// - Null values
    /// - Enum conversion (int or string for discriminators)
    /// - Optional RETURNING clause
    /// - Optional ON CONFLICT behavior
    /// </summary>
    /// <param name="table">Target database table.</param>
    /// <param name="properties">Dictionary of column names and values.</param>
    /// <param name="returningColumn">Optional column to return.</param>
    /// <param name="conflictAction">
    /// Defines behavior in case of conflict:
    /// Throw (default), DO NOTHING, or DO UPDATE.
    /// </param>
    /// <returns>A SqlStatement containing query + parameters.</returns>
    public SqlStatement Build(
        string table,
        IDictionary<string, object?> properties,
        string? returningColumn = null,
        SqlConflictAction conflictAction = SqlConflictAction.Throw)
    {
        var columns = new List<string>();
        var values = new List<string>();
        var parameters = new List<NpgsqlParameter>();

        foreach (var prop in properties)
        {
            columns.Add(prop.Key);
            values.Add("@" + prop.Key);

            parameters.Add(SqlParameterFactory.Create(prop.Key, prop.Value));
        }

        var sql = $"INSERT INTO {table} ({string.Join(", ", columns)}) " +
                  $"VALUES ({string.Join(", ", values)})";

        if (!string.IsNullOrWhiteSpace(returningColumn))
            sql += $" RETURNING {returningColumn}";
        
        sql += conflictAction switch
        {
            SqlConflictAction.Nothing => " ON CONFLICT DO NOTHING", 
            SqlConflictAction.Update => " ON CONFLICT DO UPDATE", 
            _ => string.Empty
        };

        return new SqlStatement(sql, parameters);
    }
}