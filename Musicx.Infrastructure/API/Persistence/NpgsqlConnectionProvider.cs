using System.Dynamic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Musicx.Infrastructure.Shared.Exceptions;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence;

public interface IDbConnectionProvider
{
    NpgsqlConnection CreateConnection();
    Task ExecuteTransactionAsync(params (string sql, IReadOnlyList<NpgsqlParameter> parameters)[] commands);
    Task<List<ExpandoObject>> FetchListDynamicAsync(string sql, IReadOnlyList<NpgsqlParameter> parameters);
    Task<int> Count(string table);
}

public sealed class NpgsqlConnectionProvider(
    IConfiguration configuration,
    ILoggerProvider loggerProvider) : IDbConnectionProvider
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(NpgsqlConnectionProvider));
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new NullReferenceException("Musicx database connection string not found");
    
    public NpgsqlConnection CreateConnection()
        => new(_connectionString);

    public async Task ExecuteTransactionAsync(params (string sql, IReadOnlyList<NpgsqlParameter> parameters)[] commands)
    {
        await using var conn = CreateConnection();
        await conn.OpenAsync();
        await using var transaction = await conn.BeginTransactionAsync();

        try
        {
            foreach (var (sql, parameters) in commands)
            {
                _logger.LogDebug(SqlHelper.InterpolateQuery(sql, parameters));
                var command = new NpgsqlCommand(sql, conn, transaction);
                command.Parameters.AddRange(parameters.ToArray());
                await command.ExecuteNonQueryAsync();
            }
            
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new RepositoryException($"📜❌ Could not execute batched SQL commands.", ex, _logger);
        }
    }

    public async Task<List<ExpandoObject>> FetchListDynamicAsync(string sql, IReadOnlyList<NpgsqlParameter> parameters)
    {
        _logger.LogDebug(SqlHelper.InterpolateQuery(sql, parameters));
        var results = new List<ExpandoObject>();

        try
        {
            await using var conn = CreateConnection();
            await conn.OpenAsync();

            await using var command = new NpgsqlCommand(sql, conn);
            command.Parameters.AddRange(parameters.ToArray());

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var obj = new ExpandoObject() as IDictionary<string, object?>;
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    var name = reader.GetName(i);
                    var value = await reader.IsDBNullAsync(i) ? null : reader.GetValue(i);
                    obj[name] = value;
                }

                results.Add((ExpandoObject)obj);
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical("❌ An error has occured while fetching data: " + ex.Message);
            throw;
        }
        
        return results;
    }

    public async Task<int> Count(string table)
    {
        await using var conn = CreateConnection();
        await conn.OpenAsync();
        
        var sql = $"SELECT COUNT(*) FROM {table}";
        
        await using var command = new NpgsqlCommand(sql, conn);
        _logger.LogDebug(SqlHelper.InterpolateQuery(sql, []));

        try
        {
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(await command.ExecuteScalarAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError("❌ Could not execute count command on table {table}", table);
            return -1;
        }
        
    }
}