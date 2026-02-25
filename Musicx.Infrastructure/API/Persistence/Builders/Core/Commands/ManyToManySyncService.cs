using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders.Core.Commands;

internal sealed class ManyToManySyncService
{
    /// <summary>
    /// Synchronizes a many-to-many relationship table.
    /// 
    /// Removes entries that no longer exist in the provided list.
    /// 
    /// Steps:
    /// 1. Retrieve existing relations.
    /// 2. Compute difference between existing and new values.
    /// 3. Delete obsolete relations.
    /// 
    /// This does NOT insert missing values.
    /// </summary>
    /// <param name="table">Join table name.</param>
    /// <param name="keyColumn">Foreign key column.</param>
    /// <param name="targetColumn">Related entity column.</param>
    /// <param name="keyValue">Owner entity id.</param>
    /// <param name="newValues">New set of related ids.</param>
    /// <param name="scopedColumns">
    /// Optional extra columns for scoping (multi-tenant or partitioned data).
    /// </param>
    public async Task SyncAsync(
        string table,
        string keyColumn,
        string targetColumn,
        long keyValue,
        IReadOnlyList<long> newValues,
        NpgsqlConnection connection,
        NpgsqlTransaction? transaction = null,
        IDictionary<string, object?>? scopedColumns = null)
    {
        var existing = new List<long>();
        var scopedConditions = scopedColumns?
            .Select((x, i) => $"{x.Key} = @scope{i}")
            .ToList() ?? [];

        var whereSql = $"{keyColumn} = @key";

        if (scopedConditions.Any())
        {
            whereSql += $" AND {string.Join(" AND ", scopedConditions)}";
        }

        // Step 1: Retrieve existing values from join table
        var selectSql = $"SELECT {targetColumn} FROM {table} WHERE {whereSql}";
        
        await using (var cmd = new NpgsqlCommand(selectSql, connection, transaction))
        {
            cmd.Parameters.AddWithValue("@key", keyValue);
            
            if (scopedColumns is not null)
            {
                var i = 0;

                foreach (var scopedColumn in scopedColumns)
                {
                    cmd.Parameters.AddWithValue($"@scope{i}", scopedColumn.Value ?? DBNull.Value);
                    i++;
                }
            }
            
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                existing.Add((long)reader.GetValue(0));
            }
        }
        
        // Step 2: Compute differences
        var toAdd = newValues.Except(existing).ToList();
        var toRemove = existing.Except(newValues).ToList();

        // Step 3: Delete obsolete relations
        foreach (var val in toRemove)
        {
            var deleteSql = $"DELETE FROM {table} WHERE {whereSql} AND {targetColumn} = @val";
            await using var deleteCmd = new NpgsqlCommand(deleteSql, connection, transaction);
            deleteCmd.Parameters.AddWithValue("@key", keyValue);
            
            if (scopedColumns is not null)
            {
                var i = 0;

                foreach (var scopedColumn in scopedColumns)
                {
                    deleteCmd.Parameters.AddWithValue($"@scope{i}", scopedColumn.Value ?? DBNull.Value);
                    i++;
                }
            }
            
            deleteCmd.Parameters.AddWithValue("@val", val);
            await deleteCmd.ExecuteNonQueryAsync();
        }
    }
}