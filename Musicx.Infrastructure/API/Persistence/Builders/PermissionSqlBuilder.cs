using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Specifications;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Infrastructure.API.Persistence.Columns;
using Musicx.Infrastructure.API.Persistence.Helpers;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders;

internal sealed class PermissionSqlBuilder(ILoggerProvider loggerProvider) : SqlBuilder<InPermission>
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(PermissionSqlBuilder));
    
    internal override async Task<object?> ExecuteInsert(InPermission entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        long permissionId;
        
        var createCommandSql = BuildInsert("permissions",
            new Dictionary<string, object?>
            {
                { PermissionColumns.CreatedAt, DateTime.Now },
                { PermissionColumns.UpdatedAt, DateTime.Now },
                { PermissionColumns.Name, entity.Name },
            }, PermissionColumns.Id);
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(createCommandSql.Query, createCommandSql.Parameters));
        
        await using (var cmd = new NpgsqlCommand(createCommandSql.Query, connection, transaction))
        {
            cmd.Parameters.AddRange(createCommandSql.Parameters.ToArray());
            permissionId = (long)(await cmd.ExecuteScalarAsync() ??
                             throw new NullReferenceException("Could not insert entity."));
        }
        
        _logger.LogDebug("ℹ️ Id for new entity is : " + permissionId);

        return permissionId;
    }

    internal override async Task ExecuteUpdate(InPermission entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        var updateCommandSql = BuildUpdate("permissions",
            PermissionColumns.Id,
            entity.Id,
            new Dictionary<string, object?>
            {
                { PermissionColumns.UpdatedAt, DateTime.Now },
                { PermissionColumns.Name, entity.Name },
            });
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(updateCommandSql.Query, updateCommandSql.Parameters));

        await using (var cmd = new NpgsqlCommand(updateCommandSql.Query, connection, transaction))
        {
            cmd.Parameters.AddRange(updateCommandSql.Parameters.ToArray());
            await cmd.ExecuteScalarAsync();
        }
    }

    internal override Task ExecuteUpsert(InPermission entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        throw new NotImplementedException();
    }

    internal override string BuildSelect(IQuerySpecification<InPermission>? querySpecification = null, bool distinct = false)
    {
        return distinct
            ? "SELECT DISTINCT p0.* FROM permissions p0"
            : "SELECT p0.* FROM permissions p0";
    }

    internal override string BuildGroupBy(IQuerySpecification<InPermission>? querySpecification = null)
        => $" GROUP BY p0.{PermissionColumns.Id}";
}