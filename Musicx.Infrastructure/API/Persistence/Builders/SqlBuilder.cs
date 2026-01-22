using System.Globalization;
using System.Text;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Infrastructure.API.Persistence.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders;

/// <summary>
/// Represents an SQL query and its associated parameters.
/// </summary>
internal record SqlStatement (
    string Query,
    List<NpgsqlParameter> Parameters
);

/// <summary>
/// Abstract builder class to generate SQL statements for a given input model.
/// </summary>
/// <typeparam name="T">The input model type.</typeparam>
internal abstract class SqlBuilder<T> where T : BaseInputModel
{
    /// <summary>
    /// Executes an INSERT operation for the given entity.
    /// </summary>
    internal abstract Task<object?> ExecuteInsert(T entity, 
        NpgsqlConnection connection, NpgsqlTransaction? transaction = null);
    
    /// <summary>
    /// Executes an UPDATE operation for the given entity.
    /// </summary>
    internal abstract Task ExecuteUpdate(T entity, 
        NpgsqlConnection connection, NpgsqlTransaction? transaction = null);
    
    /// <summary>
    /// Executes and UPSERT operation for the given entity.
    /// </summary>
    internal abstract Task ExecuteUpsert(T entity,
        NpgsqlConnection connection, NpgsqlTransaction? transaction = null);
    
    /// <summary>
    /// Builds a SELECT query with a given specification for joins.
    /// </summary>
    internal abstract string BuildSelect(IJoinSpecification<T>? spec = null, bool distinct = false);
    
    /// <summary>
    /// Builds a GROUP BY clause with a given specification for joins.
    /// </summary>
    internal abstract string BuildGroupBy(IJoinSpecification<T>? spec = null);

    /// <summary>
    /// Create a INSERT INTO command with a set of values.
    /// </summary>
    /// <param name="table">The database table name</param>
    /// <param name="properties">A dictionary of properties with its column name and its value</param>
    /// <param name="returningColumn">Optional column to return</param>
    /// <param name="conflictAction">Allows to do a UPDATE or NOTHING action if the entry already exists (ON CONFLICT query)</param>
    /// <returns></returns>
    internal SqlStatement BuildInsert(string table,
        IDictionary<string, object?> properties,
        string returningColumn = "",
        SqlConflictAction conflictAction = SqlConflictAction.Throw)
    {
        var keys = new List<string>();
        var values = new List<string>();
        var parameters = new List<NpgsqlParameter>();

        foreach (var property in properties)
        {
            keys.Add(property.Key);
            
            var securizedValue = "@" + property.Key;
            
            values.Add(securizedValue);

            if (property.Value is null)
            {
                parameters.Add(new NpgsqlParameter(securizedValue, DBNull.Value));
            }
            else
            {
                var actualType = Nullable.GetUnderlyingType(property.Value.GetType()) ?? property.Value.GetType();

                if (actualType.IsEnum)
                {
                    parameters.Add(property.Key.Contains("Discriminator", StringComparison.InvariantCultureIgnoreCase)
                        ? new NpgsqlParameter(securizedValue, property.Value.ToString())
                        : new NpgsqlParameter(securizedValue, (int)property.Value));
                }
                else
                {
                    parameters.Add(new NpgsqlParameter(securizedValue, property.Value));
                }
            }
        }
        
        var sql = $"INSERT INTO {table} ({string.Join(", ", keys)}) VALUES ({string.Join(", ", values)})"
                  + (string.IsNullOrWhiteSpace(returningColumn)
                      ? ""
                      : " RETURNING " + returningColumn);

        sql += conflictAction switch
        {
            SqlConflictAction.Nothing => " ON CONFLICT DO NOTHING",
            SqlConflictAction.Update => " ON CONFLICT DO UPDATE",
            _ => ""
        };
        
        return new SqlStatement(sql, parameters);
    }

    /// <summary>
    /// Creates an UPDATE statement for the specified table and entity.
    /// </summary>
    /// <param name="table">The table to update.</param>
    /// <param name="whereColumns">The columns to match the ID on.</param>
    /// <param name="whereIds">The value of the IDs to match.</param>
    /// <param name="properties">A dictionary of column names and values to update.</param>
    internal SqlStatement BuildUpdate(string table, string[] whereColumns, long[] whereIds,
        IDictionary<string, object?> properties)
    {
        var setters = new List<string>();
        var parameters = new List<NpgsqlParameter>();

        foreach (var property in properties)
        {
            if (property.Key == "Id")
            {
                continue;
            }
            
            var securizedValue = "@" + property.Key;
            setters.Add($"{property.Key} = {securizedValue}");

            if (property.Value is null)
            {
                parameters.Add(new NpgsqlParameter(securizedValue, DBNull.Value));
            }
            else
            {
                var actualType = Nullable.GetUnderlyingType(property.Value.GetType()) ?? property.Value.GetType();

                if (actualType.IsEnum)
                {
                    if (property.Key.Contains("Discriminator", StringComparison.InvariantCultureIgnoreCase))
                    {
                        parameters.Add(new NpgsqlParameter(securizedValue, property.Value.ToString()));
                    }
                    else
                    {
                        parameters.Add(new NpgsqlParameter(securizedValue, (int)property.Value));
                    }
                }
                else
                {
                    parameters.Add(new NpgsqlParameter(securizedValue, property.Value));
                }
            }
        }

        if (whereColumns.Length != whereIds.Length)
        {
            throw new ArgumentException("❌ whereColumns and whereIds must have the same number of elements.");
        }

        var whereClauses = new List<string>();

        for (int i = 0; i < whereColumns.Length; i++)
        {
            var paramName = $"@Id{i}";
            whereClauses.Add($"{whereColumns[i]} = {paramName}");
            parameters.Add(new NpgsqlParameter(paramName, whereIds[i]));
        }

        var sql = $"UPDATE {table} SET {string.Join(", ", setters)} WHERE {string.Join(" AND ", whereClauses)}";
        
        return new SqlStatement(sql, parameters);
    }

    /// <summary>
    /// Creates an UPDATE statement for the specified table and entity.
    /// </summary>
    /// <param name="table">The table to update.</param>
    /// <param name="whereColumn">The column to match the ID on.</param>
    /// <param name="whereId">The value of the ID to match.</param>
    /// <param name="properties">A dictionary of column names and values to update.</param>
    internal SqlStatement BuildUpdate(string table, string whereColumn, long whereId, 
        IDictionary<string, object?> properties)
        => BuildUpdate(table, [whereColumn], [whereId], properties);

    /// <summary>
    /// Creates an INSERT statement if the entity does not exist, else UPDATE it.
    /// </summary>
    /// <param name="table">The table to update.</param>
    /// <param name="insertProperties">A dictionary of column names and values to insert.</param>
    /// <param name="conflictColumns">The list of column that serves as identifier to check if the row already exists</param>
    /// <param name="updateProperties">A dictionary of the columns to update.</param>
    /// <param name="returningColumn">Optional column to return</param>
    /// <exception cref="ArgumentException">If conflict columns is an empty array.</exception>
    internal SqlStatement BuildUpsert(string table, IDictionary<string, object?> insertProperties,
        string[] conflictColumns, IDictionary<string, object?> updateProperties,
        string returningColumn = "")
    {
        var insert = BuildInsert(
            table, 
            insertProperties, 
            returningColumn
        );

        if (conflictColumns.Length == 0)
        {
            throw new ArgumentException("❌ ConflictColumns cannot be empty for an UPSERT.");
        }

        var setters = new List<string>();
        var updateParameters = new List<NpgsqlParameter>();

        foreach (var property in updateProperties)
        {
            var paramName = "@upd_" + property.Key;
            setters.Add($"{property.Key} = {paramName}");

            if (property.Value is null)
            {
                updateParameters.Add(new NpgsqlParameter(paramName, DBNull.Value));
            }
            else
            {
                var actualType = Nullable.GetUnderlyingType(property.Value.GetType()) ?? property.Value.GetType();

                if (actualType.IsEnum)
                {
                    updateParameters.Add(
                        property.Key.Contains("Discriminator", StringComparison.InvariantCultureIgnoreCase)
                            ? new NpgsqlParameter(paramName, property.Value.ToString())
                            : new NpgsqlParameter(paramName, (int) property.Value)
                    );
                }
                else
                {
                    updateParameters.Add(new NpgsqlParameter(paramName, property.Value));
                }
            }
        }

        var sql = insert.Query
                  + $" ON CONFLICT ({string.Join(", ", conflictColumns)})"
                  + $" DO UPDATE SET {string.Join(", ", setters)}";

        var parameters = insert.Parameters
            .Concat(updateParameters)
            .ToList();

        return new SqlStatement(sql, parameters);
    }

    /// <summary>
    /// Builds an ORDER BY clause with the given columns.
    /// </summary>
    /// <param name="columns">The columns to sort by.</param>
    internal string BuildOrderBy(OrderSpecification<T> orderSpec)
    {
        orderSpec.Validate();

        var clauses = orderSpec.ToClauses();

        if (!clauses.Any())
        {
            return string.Empty;
        }

        return $" ORDER BY {string.Join(", ", orderSpec.ToClauses().Select(c => c.Field + " " + c.Direction))}";
    }
    
    /// <summary>
    /// For a list of many-to-many values, delete those who are not existing anymore.
    /// </summary>
    /// <param name="table"></param>
    /// <param name="keyColumn"></param>
    /// <param name="targetColumn"></param>
    /// <param name="keyValue"></param>
    /// <param name="newValues"></param>
    /// <param name="connection"></param>
    /// <param name="transaction"></param>
    internal async Task DeleteMissingManyAsync(string table,
        string keyColumn, string targetColumn, long keyValue,
        IReadOnlyList<long> newValues,
        NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        var existingValues = new List<long>();
        var selectSql = $"SELECT {targetColumn} FROM {table} WHERE {keyColumn} = @key";
        await using (var cmd = new NpgsqlCommand(selectSql, connection, transaction))
        {
            cmd.Parameters.AddWithValue("@key", keyValue);
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                existingValues.Add((long)reader.GetValue(0));
            }
        }
        
        var toAdd = newValues.Except(existingValues).ToList();
        var toRemove = existingValues.Except(newValues).ToList();

        foreach (var val in toRemove)
        {
            var deleteSql = $"DELETE FROM {table} WHERE {keyColumn} = @key AND {targetColumn} = @val";
            await using var deleteCmd = new NpgsqlCommand(deleteSql, connection, transaction);
            deleteCmd.Parameters.AddWithValue("@key", keyValue);
            deleteCmd.Parameters.AddWithValue("@val", val);
            await deleteCmd.ExecuteNonQueryAsync();
        }
    }

    internal void Filter(ref string sql, IEnumerable<string> columns, 
        string filter, List<NpgsqlParameter> parameters,
        bool? filterExact = null, double? filterSimilitude = null)
    {
        if (string.IsNullOrWhiteSpace(filter) || !columns.Any())
        {
            return;
        }

        var paramName = "@filter";
        parameters.Add(new NpgsqlParameter(paramName, filter));

        string condition;
        
        if (filterExact is not null && filterExact.Value)
        {
            condition = string.Join(" OR ", columns.Select(c => $"{c} ILIKE {paramName}"));
        }
        else
        {
            // Approximate match via similarity()
            var similitude = (filterSimilitude ?? 0.4).ToString(CultureInfo.InvariantCulture);
            condition = string.Join(" OR ", columns.Select(c => $"similarity({c}, {paramName}) > {similitude}"));
        }
        
        sql += $" WHERE ({condition})";
    }
    
    internal void Filter(ref string sql, string column, string filter, List<NpgsqlParameter> parameters,
        bool? filterExact = null, double? filterSimilitude = null)
        => Filter(ref sql, [column], filter, parameters, filterExact, filterSimilitude);
}