using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Genre;
using Musicx.Infrastructure.API.Persistence.Columns.Genre;
using Musicx.Infrastructure.API.Persistence.Specifications.Genre;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders.Genre;

internal sealed class GenreSqlBuilder(ILoggerProvider loggerProvider) : SqlBuilder<InGenre>
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(GenreSqlBuilder));
    
    internal override async Task<object?> ExecuteInsert(InGenre entity, NpgsqlConnection connection, 
        NpgsqlTransaction? transaction = null)
    {
        long genreId;

        var createCommandSql = InsertBuilder.Build("genres",
            new Dictionary<string, object?>
            {
                { GenreColumns.CreatedAt, DateTime.Now },
                { GenreColumns.UpdatedAt, DateTime.Now },
                { GenreColumns.CanonicalName, entity.CanonicalName },
                { GenreColumns.Color, entity.Color },
                { GenreColumns.Confidence, entity.Confidence },
                { GenreColumns.CountryOrigin, entity.CountryOrigin },
                { GenreColumns.Description, entity.Description },
                { GenreColumns.EraStart, entity.EraStart },
                { GenreColumns.EraEnd, entity.EraEnd },
                { GenreColumns.IsVisible, entity.IsVisible },
                { GenreColumns.Metadata, entity.Metadata },
                { GenreColumns.ShortName, entity.ShortName },
                { GenreColumns.Taggable, entity.IsTaggable },
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

        return genreId;
    }

    internal override async Task ExecuteUpdate(InGenre entity, 
        NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        var updateCommandSql = UpdateBuilder.Build("genres",
            new Dictionary<string, object?>
            {
                { GenreColumns.UpdatedAt, DateTime.Now },
                { GenreColumns.CanonicalName, entity.CanonicalName },
                { GenreColumns.Color, entity.Color },
                { GenreColumns.Confidence, entity.Confidence },
                { GenreColumns.CountryOrigin, entity.CountryOrigin },
                { GenreColumns.Description, entity.Description },
                { GenreColumns.EraStart, entity.EraStart },
                { GenreColumns.EraEnd, entity.EraEnd },
                { GenreColumns.IsVisible, entity.IsVisible },
                { GenreColumns.Metadata, entity.Metadata },
                { GenreColumns.ShortName, entity.ShortName },
                { GenreColumns.Taggable, entity.IsTaggable },
                { GenreColumns.Type, entity.Type }
            },
            new Dictionary<string, object?>
            {
                { GenreColumns.Id, entity.Id }
            }
        );
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(updateCommandSql.Query, updateCommandSql.Parameters));

        await using (var cmd = new NpgsqlCommand(updateCommandSql.Query, connection, transaction))
        {
            cmd.Parameters.AddRange(updateCommandSql.Parameters.ToArray());
            await cmd.ExecuteScalarAsync();
        }
    }

    internal override async Task ExecuteUpsert(InGenre entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
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

    internal override string BuildSelect(IJoinSpecification<InGenre>? querySpecification = null, bool distinct = false)
    {
        if (querySpecification is not GenreJoinSpecification genreQuerySpecification)
        {
            return distinct 
                ? "SELECT DISTINCT g0.* FROM genres g0"
                : "SELECT g0.* FROM genres g0";
        }

        var selects = new List<string> { "g0.*" };
        var joins = new List<string>();

        if (genreQuerySpecification.IncludeAliases)
        {
            selects.Add($"(SELECT json_agg(gal.*) FROM genre_alias gal " +
                        $"WHERE g0.{GenreColumns.Id} = gal.{GenreAliasColumns.GenreId}) AS aliases");
        }

        if (genreQuerySpecification.IncludeChildren)
        {
            selects.Add($"(SELECT json_agg(jsonb_build_object('relation', cg.*, 'depth', chgc.{GenreClosureColumns.Depth})) FROM genre_closure chgc " +
                        $"JOIN genres cg ON chgc.{GenreClosureColumns.DescendantId} = cg.{GenreColumns.Id} " +
                        $"WHERE g0.{GenreColumns.Id} = chgc.{GenreClosureColumns.AncestorId}) AS children");
        }
        
        if (genreQuerySpecification.IncludeParents)
        {
            selects.Add($"(SELECT json_agg(jsonb_build_object('relation', pg.*, 'depth', pagc.{GenreClosureColumns.Depth})) FROM genre_closure pagc " +
                        $"JOIN genres pg ON pagc.{GenreClosureColumns.AncestorId} = pg.{GenreColumns.Id} " +
                        $"WHERE g0.{GenreColumns.Id} = pagc.{GenreClosureColumns.DescendantId}) AS parents");
        }

        if (genreQuerySpecification.IncludeRelations)
        {
            selects.Add($"(SELECT json_agg(jsonb_build_object('relation', gr.*, 'related_genre', rg.*)) FROM genre_relation gr " +
                        $"JOIN genres rg ON gr.{GenreRelationColumns.ToGenreId} = rg.{GenreColumns.Id} " +
                        $"WHERE gr.{GenreRelationColumns.FromGenreId} = g0.{GenreColumns.Id}) AS relations");
        }

        return distinct
            ? $"SELECT DISTINCT {string.Join(", ", selects)} FROM genres g0 {string.Join(" ", joins)}"
            : $"SELECT {string.Join(", ", selects)} FROM genres g0 {string.Join(" ", joins)}";
    }

    internal override string BuildGroupBy(IJoinSpecification<InGenre>? spec = null)
        => $" GROUP BY g0.{GenreColumns.Id}";
}