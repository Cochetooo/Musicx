using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Security;
using Musicx.Infrastructure.API.Persistence.Columns.Security;
using Musicx.Infrastructure.API.Persistence.Helpers;
using Musicx.Infrastructure.API.Persistence.Specifications.Security;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders.Security;

internal sealed class RoleSqlBuilder(ILoggerProvider loggerProvider) : SqlBuilder<InRole>
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(RoleSqlBuilder));
    
    internal override async Task<object?> ExecuteInsert(InRole entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        long roleId;

        var createCommandSql = InsertBuilder.Build("roles",
            new Dictionary<string, object?>
            {
                { RoleColumns.CreatedAt, DateTime.Now },
                { RoleColumns.UpdatedAt, DateTime.Now },
                { RoleColumns.Name, entity.Name },
            }, RoleColumns.Id);
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(createCommandSql.Query, createCommandSql.Parameters));
        
        await using (var cmd = new NpgsqlCommand(createCommandSql.Query, connection, transaction))
        {
            cmd.Parameters.AddRange(createCommandSql.Parameters.ToArray());
            roleId = (long)(await cmd.ExecuteScalarAsync() ??
                             throw new NullReferenceException("Could not insert entity."));
        }
        
        _logger.LogDebug("ℹ️ Id for new entity is : " + roleId);

        if (null != entity.PermissionIds)
        {
            foreach (var permission in entity.PermissionIds)
            {
                var permissionSql = InsertBuilder.Build("role_permission",
                    new Dictionary<string, object?>
                    {
                        { RolePermissionColumns.RoleId, roleId },
                        { RolePermissionColumns.PermissionId, permission }
                    });
                
                _logger.LogDebug(SqlHelper.InterpolateQuery(permissionSql.Query, permissionSql.Parameters));

                await using var cmd = new NpgsqlCommand(permissionSql.Query, connection, transaction);
                cmd.Parameters.AddRange(permissionSql.Parameters.ToArray());
                await cmd.ExecuteNonQueryAsync();
            }
        }

        return roleId;
    }

    internal override async Task ExecuteUpdate(InRole entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        var updateCommandSql = UpdateBuilder.Build("roles",
            new Dictionary<string, object?>
            {
                { RoleColumns.UpdatedAt, DateTime.Now },
                { RoleColumns.Name, entity.Name },
            },
            new Dictionary<string, object?>
            {
                { RoleColumns.Id, entity.Id }
            }
        );
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(updateCommandSql.Query, updateCommandSql.Parameters));

        await using (var cmd = new NpgsqlCommand(updateCommandSql.Query, connection, transaction))
        {
            cmd.Parameters.AddRange(updateCommandSql.Parameters.ToArray());
            await cmd.ExecuteScalarAsync();
        }
        
        if (null != entity.PermissionIds)
        {
            foreach (var permission in entity.PermissionIds)
            {
                var permissionSql = InsertBuilder.Build("role_permission",
                    new Dictionary<string, object?>
                    {
                        { RolePermissionColumns.RoleId, entity.Id },
                        { RolePermissionColumns.PermissionId, permission }
                    },
                    conflictAction: SqlConflictAction.Nothing
                );
                
                _logger.LogDebug(SqlHelper.InterpolateQuery(permissionSql.Query, permissionSql.Parameters));

                await using var cmd = new NpgsqlCommand(permissionSql.Query, connection, transaction);
                cmd.Parameters.AddRange(permissionSql.Parameters.ToArray());
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }

    internal override async Task ExecuteUpsert(InRole entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        if (0 == entity.Id)
        {
            var result = await ExecuteInsert(entity, connection, transaction);
            if (result is long id)
            {
                entity.Id = id;
            }
            return;
        }

        await ExecuteUpdate(entity, connection, transaction);
    }

    internal override string BuildSelect(IJoinSpecification<InRole>? querySpecification = null, bool distinct = false)
    {
        if (querySpecification is not RoleJoinSpecification roleQuerySpecification)
        {
            return distinct
                ? "SELECT DISTINCT r0.* FROM roles r0"
                : "SELECT r0.* FROM roles r0";
        }

        var selects = new List<string> { "r0.*" };
        var joins = new List<string>();

        if (roleQuerySpecification.IncludePermissions)
        {
            selects.Add($"(SELECT json_agg(p.* ORDER BY p.{PermissionColumns.Name}) FROM role_permission rp " +
                        $"JOIN permissions p ON rp.{RolePermissionColumns.PermissionId} = p.{PermissionColumns.Id} " +
                        $"WHERE rp.{RolePermissionColumns.RoleId} = r0.{RoleColumns.Id}) AS permissions");
        }

        return distinct
            ? $"SELECT DISTINCT {string.Join(", ", selects)} FROM roles r0 {string.Join(" ", joins)}"
            : $"SELECT {string.Join(", ", selects)} FROM roles r0 {string.Join(" ", joins)}";
    }

    internal override string BuildGroupBy(IJoinSpecification<InRole>? querySpecification = null)
        => $" GROUP BY r0.{RoleColumns.Id}";
}