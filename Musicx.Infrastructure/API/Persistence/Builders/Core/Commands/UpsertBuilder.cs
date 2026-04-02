using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders.Core.Commands;

internal sealed class UpsertBuilder
{
    private readonly InsertBuilder _insertBuilder = new();
    
    /// <summary>
    /// Builds a PostgreSQL UPSERT statement.
    /// 
    /// Generates:
    /// INSERT INTO ...
    /// ON CONFLICT (columns)
    /// DO UPDATE SET ...
    /// </summary>
    public SqlStatement Build(
        string table,
        IDictionary<string, object?> insertProperties,
        string[]? conflictColumns,
        IDictionary<string, object?> updateProperties,
        string? conflictConstraint = null,
        string? returningColumn = null)
    {
        if ((conflictColumns is null || conflictColumns.Length == 0) && string.IsNullOrWhiteSpace(conflictConstraint))
            throw new ArgumentException("Conflict target cannot be empty for an UPSERT.");

        // 1️⃣ Build base INSERT
        var insertStatement = _insertBuilder.Build(
            table,
            insertProperties,
            returningColumn
        );

        var updateSetters = new List<string>();
        var updateParameters = new List<NpgsqlParameter>();

        // 2️⃣ Build UPDATE SET clause
        foreach (var prop in updateProperties)
        {
            var paramName = "upd_" + prop.Key;

            updateSetters.Add($"{prop.Key} = @{paramName}");
            updateParameters.Add(
                SqlParameterFactory.Create(paramName, prop.Value)
            );
        }

        // 3️⃣ Compose final SQL
        var sql =
            insertStatement.Query +
            (string.IsNullOrWhiteSpace(conflictConstraint)
                ? $" ON CONFLICT ({string.Join(", ", conflictColumns!)})"
                : $" ON CONFLICT ON CONSTRAINT {conflictConstraint}") +
            $" DO UPDATE SET {string.Join(", ", updateSetters)}";

        // 4️⃣ Merge parameters (INSERT + UPDATE)
        var parameters = insertStatement.Parameters
            .Concat(updateParameters)
            .ToList();

        return new SqlStatement(sql, parameters);
    }
}