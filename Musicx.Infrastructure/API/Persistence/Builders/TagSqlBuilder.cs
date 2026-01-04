using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Infrastructure.API.Persistence.Columns;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders;

internal sealed class TagSqlBuilder(ILoggerProvider loggerProvider) : SqlBuilder<InTag>
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(TagSqlBuilder));

    internal override async Task<object?> ExecuteInsert(InTag entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        long tagId;

        var createCommandSql = BuildInsert("tags",
            new Dictionary<string, object?>
            {
                { TagColumns.CreatedAt, DateTime.Now },
                { TagColumns.UpdatedAt, DateTime.Now },
                { TagColumns.Name, entity.Name },
            }, TagColumns.Id);
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(createCommandSql.Query, createCommandSql.Parameters));
        
        await using (var cmd = new NpgsqlCommand(createCommandSql.Query, connection, transaction))
        {
            cmd.Parameters.AddRange(createCommandSql.Parameters.ToArray());
            tagId = (long)(await cmd.ExecuteScalarAsync() ??
                            throw new NullReferenceException("Could not insert entity."));
        }
        
        _logger.LogDebug("ℹ️ Id for new entity is : " + tagId);

        return tagId;
    }

    internal override async Task ExecuteUpdate(InTag entity, NpgsqlConnection connection,
        NpgsqlTransaction? transaction = null)
    {
        /*
         * Actually should NEVER happen as a tag is unique by its name,
         * But we keep this function just in case we have to make a mass-edit thing someday.
         */
        
        var updateCommandSql = BuildUpdate("tags",
            TagColumns.Id,
            entity.Id,
            new Dictionary<string, object?>
            {
                { TagColumns.UpdatedAt, DateTime.Now },
                { TagColumns.Name, entity.Name },
            });

        _logger.LogDebug(SqlHelper.InterpolateQuery(updateCommandSql.Query, updateCommandSql.Parameters));

        await using (var cmd = new NpgsqlCommand(updateCommandSql.Query, connection, transaction))
        {
            cmd.Parameters.AddRange(updateCommandSql.Parameters.ToArray());
            await cmd.ExecuteScalarAsync();
        }
    }

    internal override Task ExecuteUpsert(InTag entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        throw new NotImplementedException();
    }

    internal override string BuildSelect(IQuerySpecification<InTag>? querySpecification = null, bool distinct = false)
        => distinct
            ? "SELECT DISTINCT t0.* FROM tags t0"
            : "SELECT t0.* FROM tags t0";
    
    internal override string BuildGroupBy(IQuerySpecification<InTag>? querySpecification = null)
        => $"GROUP BY t0.{TagColumns.Id}";
}