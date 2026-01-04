using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Auth;
using Musicx.Application.Api.Interfaces.Specifications;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Infrastructure.API.Persistence.Columns;
using Musicx.Infrastructure.API.Persistence.Helpers;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders;

internal sealed class UserSqlBuilder(
    ILoggerProvider loggerProvider) : SqlBuilder<InUser>
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(UserSqlBuilder));
    
    internal override async Task<object?> ExecuteInsert(InUser entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        long userId;

        var createCommandSql = BuildInsert("users",
            new Dictionary<string, object?>
            {
                { UserColumns.CreatedAt, DateTime.Now },
                { UserColumns.UpdatedAt, DateTime.Now },
                { UserColumns.Biography, entity.Biography },
                { UserColumns.BirthDate, entity.BirthDate },
                { UserColumns.Email, entity.Email },
                { UserColumns.EmailConfirmed, entity.EmailConfirmed },
                { UserColumns.Name, entity.Name },
                { UserColumns.PasswordHash, entity.PasswordHash },
                { UserColumns.PasswordSalt, entity.PasswordSalt },
                { UserColumns.GoogleId, entity.GoogleId },
                { UserColumns.LastFmUsername, entity.LastFmUsername },
                { UserColumns.PrefDarkMode, entity.PrefDarkMode },
                { UserColumns.PrefRatingMode, entity.PrefRatingMode },
                { UserColumns.PrefShowRatings, entity.PrefShowRatings },
                { UserColumns.PrefSimpleGenre, entity.PrefSimpleGenre },
                { UserColumns.PictureUrl, entity.PictureUrl }
            }, UserColumns.Id);
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(createCommandSql.Query, createCommandSql.Parameters));
        
        await using (var cmd = new NpgsqlCommand(createCommandSql.Query, connection, transaction))
        {
            cmd.Parameters.AddRange(createCommandSql.Parameters.ToArray());
            userId = (long)(await cmd.ExecuteScalarAsync() ??
                             throw new NullReferenceException("Could not insert entity."));
        }
        
        _logger.LogDebug("ℹ️ Id for new entity is : " + userId);

        if (null != entity.RoleIds)
        {
            foreach (var role in entity.RoleIds)
            {
                var roleSql = BuildInsert("user_role",
                    new Dictionary<string, object?>
                    {
                        { UserRoleColumns.UserId, userId },
                        { UserRoleColumns.RoleId, role }
                    });
                
                _logger.LogDebug(SqlHelper.InterpolateQuery(roleSql.Query, roleSql.Parameters));

                await using var cmd = new NpgsqlCommand(roleSql.Query, connection, transaction);
                cmd.Parameters.AddRange(roleSql.Parameters.ToArray());
                await cmd.ExecuteNonQueryAsync();
            }
        }
        
        /*
         * There is a high chance that other relations such as rating, tags etc.
         * will never happen during a user insert, so we don't need to handle it
         * unless a future use-case will need it.
         */

        return userId;
    }

    internal override async Task ExecuteUpdate(InUser entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        var updateCommandSql = BuildUpdate("users",
            UserColumns.Id,
            entity.Id,
            new Dictionary<string, object?>
            {
                { UserColumns.UpdatedAt, DateTime.Now },
                { UserColumns.Biography, entity.Biography },
                { UserColumns.BirthDate, entity.BirthDate },
                { UserColumns.Email, entity.Email },
                { UserColumns.EmailConfirmed, entity.EmailConfirmed },
                { UserColumns.Name, entity.Name },
                { UserColumns.PasswordHash, entity.PasswordHash },
                { UserColumns.PasswordSalt, entity.PasswordSalt },
                { UserColumns.GoogleId, entity.GoogleId },
                { UserColumns.LastFmUsername, entity.LastFmUsername },
                { UserColumns.PrefDarkMode, entity.PrefDarkMode },
                { UserColumns.PrefRatingMode, entity.PrefRatingMode },
                { UserColumns.PrefShowRatings, entity.PrefShowRatings },
                { UserColumns.PrefSimpleGenre, entity.PrefSimpleGenre },
                { UserColumns.PictureUrl, entity.PictureUrl }
            });
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(updateCommandSql.Query, updateCommandSql.Parameters));

        await using (var cmd = new NpgsqlCommand(updateCommandSql.Query, connection, transaction))
        {
            cmd.Parameters.AddRange(updateCommandSql.Parameters.ToArray());
            await cmd.ExecuteNonQueryAsync();
        }
        
        if (null != entity.RoleIds)
        {
            foreach (var role in entity.RoleIds)
            {
                var roleSql = BuildInsert("user_role",
                    new Dictionary<string, object?>
                    {
                        { UserRoleColumns.UserId, entity.Id },
                        { UserRoleColumns.RoleId, role }
                    },
                    conflictAction: SqlConflictAction.Nothing);
                
                _logger.LogDebug(SqlHelper.InterpolateQuery(roleSql.Query, roleSql.Parameters));

                await using var cmd = new NpgsqlCommand(roleSql.Query, connection, transaction);
                cmd.Parameters.AddRange(roleSql.Parameters.ToArray());
                await cmd.ExecuteNonQueryAsync();
            }
        }
        
        /*
         * Similarly to Insert, there is a high change that editing a user
         * will never be on the same endpoint as updating tags, ratings etc.
         * So we won't handle it there.
         */
    }

    internal override Task ExecuteUpsert(InUser entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        throw new NotImplementedException();
    }

    internal override string BuildSelect(IQuerySpecification<InUser>? querySpecification = null, bool distinct = false)
    {
        if (querySpecification is not UserQuerySpecification userQuerySpecification)
        {
            return distinct
                ? "SELECT DISTINCT u0.* FROM users u0"
                : "SELECT u0.* FROM users u0";
        }

        var selects = new List<string> { "u0.*" };
        var joins = new List<string>();

        if (userQuerySpecification.IncludeRoles)
        {
            selects.Add("(SELECT json_agg(r.*) FROM user_role ur " +
                        $"JOIN roles r ON ur.{UserRoleColumns.RoleId} = r.{RoleColumns.Id} " +
                        $"WHERE ur.{UserRoleColumns.UserId} = u0.{UserColumns.Id}) AS roles");
        }

        return distinct
            ? $"SELECT DISTINCT {string.Join(", ", selects)} FROM users u0 {string.Join(" ", joins)}"
            : $"SELECT {string.Join(", ", selects)} FROM users u0 {string.Join(" ", joins)}";
    }

    internal override string BuildGroupBy(IQuerySpecification<InUser>? querySpecification = null)
        => $" GROUP BY u0.{UserColumns.Id}";
}