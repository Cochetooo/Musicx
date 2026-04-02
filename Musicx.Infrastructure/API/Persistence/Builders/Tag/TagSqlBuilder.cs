using Microsoft.Extensions.Logging;
using Musicx.Application.API.Persistence.Queries;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Tag;
using Musicx.Infrastructure.API.Persistence.Columns.Tag;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders.Tag;

internal sealed class TagSqlBuilder(ILoggerProvider loggerProvider) : SqlBuilder<InTag>
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(TagSqlBuilder));

    internal override async Task<object?> ExecuteInsert(InTag entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        long tagId;

        var createCommandSql = InsertBuilder.Build("tags",
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
        
        var updateCommandSql = UpdateBuilder.Build("tags",
            new Dictionary<string, object?>
            {
                { TagColumns.UpdatedAt, DateTime.Now },
                { TagColumns.Name, entity.Name },
            },
            new Dictionary<string, object?>
            {
                { TagColumns.Id, entity.Id }
            }
        );

        _logger.LogDebug(SqlHelper.InterpolateQuery(updateCommandSql.Query, updateCommandSql.Parameters));

        await using (var cmd = new NpgsqlCommand(updateCommandSql.Query, connection, transaction))
        {
            cmd.Parameters.AddRange(updateCommandSql.Parameters.ToArray());
            await cmd.ExecuteScalarAsync();
        }
    }

    internal override async Task ExecuteUpsert(InTag entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
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

    internal override string BuildSelect(IJoinSpecification<InTag>? querySpecification = null, bool distinct = false)
        => distinct
            ? "SELECT DISTINCT t0.* FROM tags t0"
            : "SELECT t0.* FROM tags t0";
    
    internal override string BuildGroupBy(IJoinSpecification<InTag>? querySpecification = null)
        => $"GROUP BY t0.{TagColumns.Id}";
    
    internal override (string Sql, List<NpgsqlParameter> Parameters) BuildFilteredQuery(
        IFindQuery<InTag> query,
        IJoinSpecification<InTag>? joinSpec,
        OrderSpecification<InTag>? orderSpec,
        PagingOptions? pagingOptions,
        bool countOnly)
    {
        if (query is not TagFindQuery typedQuery)
        {
            return (string.Empty, []);
        }

        return BuildDefaultFilteredQuery(
            typedQuery,
            joinSpec,
            orderSpec,
            pagingOptions,
            countOnly,
            [$"t0.{TagColumns.Name}"],
            $"t0.{TagColumns.Name}");
    }
}