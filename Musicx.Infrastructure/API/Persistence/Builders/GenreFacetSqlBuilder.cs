using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Infrastructure.API.Persistence.Columns;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders;

internal sealed class GenreFacetSqlBuilder(ILoggerProvider loggerProvider) : SqlBuilder<InGenreFacet>
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(GenreFacetSqlBuilder));

    internal override async Task<object?> ExecuteInsert(InGenreFacet entity, NpgsqlConnection connection,
        NpgsqlTransaction? transaction = null)
    {
        var createCommandSql = BuildInsert("genre_facet",
            new Dictionary<string, object?>
            {
                { GenreFacetColumns.GenreId, entity.GenreId },
                { GenreFacetColumns.FacetId, entity.FacetId },
                { GenreFacetColumns.CreatedAt, DateTime.Now },
                { GenreFacetColumns.UpdatedAt, DateTime.Now },
                { GenreFacetColumns.Confidence, entity.Confidence },
                { GenreFacetColumns.Metadata, entity.Metadata },
                { GenreFacetColumns.Value, entity.Value }
            });
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(createCommandSql.Query, createCommandSql.Parameters));

        await using var cmd = new NpgsqlCommand(createCommandSql.Query, connection, transaction);
        cmd.Parameters.AddRange(createCommandSql.Parameters.ToArray());
        await cmd.ExecuteNonQueryAsync();

        return null;
    }

    internal override async Task ExecuteUpdate(InGenreFacet entity, NpgsqlConnection connection,
        NpgsqlTransaction? transaction = null)
    {
        var updateCommandSql = BuildUpdate("genre_facet",
            [GenreFacetColumns.GenreId, GenreFacetColumns.FacetId],
            [entity.GenreId, entity.FacetId],
            new Dictionary<string, object?>
            {
                { GenreFacetColumns.UpdatedAt, DateTime.Now },
                { GenreFacetColumns.Confidence, entity.Confidence },
                { GenreFacetColumns.Metadata, entity.Metadata },
                { GenreFacetColumns.Value, entity.Value }
            });
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(updateCommandSql.Query, updateCommandSql.Parameters));

        await using var cmd = new NpgsqlCommand(updateCommandSql.Query, connection, transaction);
        cmd.Parameters.AddRange(updateCommandSql.Parameters.ToArray());
        await cmd.ExecuteNonQueryAsync();
    }

    internal override string BuildSelect(IQuerySpecification<InGenreFacet>? querySpecification = null,
        bool distinct = false)
        => distinct
            ? "SELECT DISTINCT gf0.* FROM genre_facet gf0 " +
              $"JOIN genres g0 ON gf0.{GenreFacetColumns.GenreId} = g0.{GenreColumns.Id} " +
              $"JOIN facets f0 on gf0.{GenreFacetColumns.FacetId} = f0.{FacetColumns.Id} "
            : "SELECT gf0.* FROM genre_facet gf0 " +
              $"JOIN genres g0 ON gf0.{GenreFacetColumns.GenreId} = g0.{GenreColumns.Id} " +
              $"JOIN facets f0 on gf0.{GenreFacetColumns.FacetId} = f0.{FacetColumns.Id} ";

    internal override string BuildGroupBy(IQuerySpecification<InGenreFacet>? spec = null)
        => string.Empty;
}