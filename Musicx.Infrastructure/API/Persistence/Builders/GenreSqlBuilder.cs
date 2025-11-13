using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Specifications;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Infrastructure.API.Persistence.Columns;
using Musicx.Infrastructure.API.Persistence.Helpers;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders;

internal sealed class GenreSqlBuilder(ILoggerProvider loggerProvider) : SqlBuilder<InGenre>
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(GenreSqlBuilder));
    
    internal override async Task<object?> ExecuteInsert(InGenre entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        long genreId;

        var createCommandSql = BuildInsert("genres",
            new Dictionary<string, object?>
            {
                { GenreColumns.CreatedAt, DateTime.Now },
                { GenreColumns.UpdatedAt, DateTime.Now },
                { GenreColumns.Color, entity.Color },
                { GenreColumns.Description, entity.Description },
                { GenreColumns.IsVisible, entity.IsVisible },
                { GenreColumns.Name, entity.Name },
                { GenreColumns.Type, entity.Type }
            },
            returningColumn: GenreColumns.Id);
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(createCommandSql.Query, createCommandSql.Parameters));

        await using (var cmd = new NpgsqlCommand(createCommandSql.Query, connection, transaction))
        {
            cmd.Parameters.AddRange(createCommandSql.Parameters.ToArray());
            genreId = (long) (await cmd.ExecuteScalarAsync() ??
                              throw new NullReferenceException("Could not insert entity"));
        }
        
        _logger.LogDebug("ℹ️ Id for new entity is : " + genreId);
        
        /*
         * NOTE : It is technically impossible to generate Children when a new genre is created,
         * Thus why we don't handle ChildIds here.
         */

        if (null != entity.ParentIds)
        {
            foreach (var parent in entity.ParentIds)
            {
                var parentSql = BuildInsert("childrengenre_parentgenre",
                    new Dictionary<string, object?>
                    {
                        { ChildrenGenreParentGenreColumns.ChildId, genreId },
                        { ChildrenGenreParentGenreColumns.ParentId, parent }
                    });
                
                _logger.LogDebug(SqlHelper.InterpolateQuery(parentSql.Query, parentSql.Parameters));
                
                await using var cmd = new NpgsqlCommand(parentSql.Query, connection, transaction);
                cmd.Parameters.AddRange(parentSql.Parameters.ToArray());
                await cmd.ExecuteNonQueryAsync();
            }
        }

        return genreId;
    }

    internal override async Task ExecuteUpdate(InGenre entity, 
        NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        var updateCommandSql = BuildUpdate("genres",
            GenreColumns.Id,
            entity.Id,
            new Dictionary<string, object?>
            {
                { GenreColumns.UpdatedAt, DateTime.Now },
                { GenreColumns.Color, entity.Color },
                { GenreColumns.Description, entity.Description },
                { GenreColumns.IsVisible, entity.IsVisible },
                { GenreColumns.Name, entity.Name },
                { GenreColumns.Type, entity.Type }
            });
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(updateCommandSql.Query, updateCommandSql.Parameters));

        await using (var cmd = new NpgsqlCommand(updateCommandSql.Query, connection, transaction))
        {
            cmd.Parameters.AddRange(updateCommandSql.Parameters.ToArray());
            await cmd.ExecuteScalarAsync();
        }

        if (null != entity.ParentIds)
        {
            foreach (var parent in entity.ParentIds)
            {
                var parentSql = BuildInsert("childrengenre_parentgenre",
                    new Dictionary<string, object?>
                    {
                        { ChildrenGenreParentGenreColumns.ChildId, entity.Id },
                        { ChildrenGenreParentGenreColumns.ParentId, parent }
                    },
                    conflictAction: SqlConflictAction.Nothing
                );
                
                _logger.LogDebug(SqlHelper.InterpolateQuery(parentSql.Query, parentSql.Parameters));
                
                await using var cmd = new NpgsqlCommand(parentSql.Query, connection, transaction);
                cmd.Parameters.AddRange(parentSql.Parameters.ToArray());
                await cmd.ExecuteNonQueryAsync();
            }
        } 
    }

    internal override string BuildSelect(IQuerySpecification<InGenre>? querySpecification = null, bool distinct = false)
    {
        if (querySpecification is not GenreQuerySpecification genreQuerySpecification)
        {
            return distinct 
                ? "SELECT DISTINCT g0.* FROM genres g0"
                : "SELECT g0.* FROM genres g0";
        }

        var selects = new List<string> { "g0.*" };
        var joins = new List<string>();

        if (genreQuerySpecification.IncludeChildren)
        {
            selects.Add($"(SELECT json_agg(cg.*) FROM childrengenre_parentgenre cgpg " +
                        $"INNER JOIN genres cg ON cgpg.{ChildrenGenreParentGenreColumns.ChildId} = cg.{GenreColumns.Id} " +
                        $"WHERE g0.{GenreColumns.Id} = cgpg.{ChildrenGenreParentGenreColumns.ParentId}) AS children");
        }
        
        if (genreQuerySpecification.IncludeParents)
        {
            selects.Add($"(SELECT json_agg(pg.*) FROM childrengenre_parentgenre pgcg " +
                        $"INNER JOIN genres pg ON pgcg.{ChildrenGenreParentGenreColumns.ParentId} = pg.{GenreColumns.Id} " +
                        $"WHERE g0.{GenreColumns.Id} = pgcg.{ChildrenGenreParentGenreColumns.ChildId}) AS parents");
        }

        return distinct
            ? $"SELECT DISTINCT {string.Join(", ", selects)} FROM genres g0 {string.Join(" ", joins)}"
            : $"SELECT {string.Join(", ", selects)} FROM genres g0 {string.Join(" ", joins)}";
    }

    internal override string BuildGroupBy(IQuerySpecification<InGenre>? spec = null)
        => $" GROUP BY g0.{GenreColumns.Id}";
}