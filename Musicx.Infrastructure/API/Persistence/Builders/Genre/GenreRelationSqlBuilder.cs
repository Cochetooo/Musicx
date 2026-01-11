using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Genre;
using Musicx.Infrastructure.API.Persistence.Columns.Genre;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders.Genre;

internal sealed class GenreRelationSqlBuilder(ILoggerProvider loggerProvider) : SqlBuilder<InGenreRelation>
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(GenreRelationSqlBuilder));

    internal override async Task<object?> ExecuteInsert(InGenreRelation entity, NpgsqlConnection connection,
        NpgsqlTransaction? transaction = null)
    {
        var createCommandSql = BuildInsert("genre_relation",
            new Dictionary<string, object?>
            {
                { GenreRelationColumns.FromGenreId, entity.FromGenreId },
                { GenreRelationColumns.ToGenreId, entity.ToGenreId },
                { GenreRelationColumns.CreatedAt, DateTime.Now },
                { GenreRelationColumns.UpdatedAt, DateTime.Now },
                { GenreRelationColumns.Metadata, entity.Metadata },
                { GenreRelationColumns.Type, entity.Type },
                { GenreRelationColumns.Weight, entity.Weight }
            });
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(createCommandSql.Query, createCommandSql.Parameters));

        await using var cmd = new NpgsqlCommand(createCommandSql.Query, connection, transaction);
        cmd.Parameters.AddRange(createCommandSql.Parameters.ToArray());
        await cmd.ExecuteNonQueryAsync();

        return null;
    }

    internal override async Task ExecuteUpdate(InGenreRelation entity, NpgsqlConnection connection,
        NpgsqlTransaction? transaction = null)
    {
        var updateCommandSql = BuildUpdate("genre_relation",
            [GenreRelationColumns.FromGenreId, GenreRelationColumns.ToGenreId],
            [entity.FromGenreId, entity.ToGenreId],
            new Dictionary<string, object?>
            {
                { GenreRelationColumns.UpdatedAt, DateTime.Now },
                { GenreRelationColumns.Metadata, entity.Metadata },
                { GenreRelationColumns.Type, entity.Type },
                { GenreRelationColumns.Weight, entity.Weight }
            });
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(updateCommandSql.Query, updateCommandSql.Parameters));

        await using var cmd = new NpgsqlCommand(updateCommandSql.Query, connection, transaction);
        cmd.Parameters.AddRange(updateCommandSql.Parameters.ToArray());
        await cmd.ExecuteNonQueryAsync();
    }

    internal override Task ExecuteUpsert(InGenreRelation entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        throw new NotImplementedException();
    }

    internal override string BuildSelect(IJoinSpecification<InGenreRelation>? querySpecification = null,
        bool distinct = false)
        => distinct
            ? "SELECT DISTINCT gr0.* FROM genre_relation gr0 " +
              $"JOIN genres fg0 ON gr0.{GenreRelationColumns.FromGenreId} = fg0.{GenreColumns.Id} " +
              $"JOIN genres tg0 on gr0.{GenreRelationColumns.ToGenreId} = tg0.{GenreColumns.Id} "
            : "SELECT gr0.* FROM genre_relation gr0 " +
              $"JOIN genres fg0 ON gr0.{GenreRelationColumns.FromGenreId} = fg0.{GenreColumns.Id} " +
              $"JOIN genres tg0 on gr0.{GenreRelationColumns.ToGenreId} = tg0.{GenreColumns.Id} ";

    internal override string BuildGroupBy(IJoinSpecification<InGenreRelation>? spec = null)
        => string.Empty;
}