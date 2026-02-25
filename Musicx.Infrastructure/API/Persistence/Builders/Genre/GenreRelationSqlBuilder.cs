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
        => throw new NotImplementedException();

    internal override async Task ExecuteUpdate(InGenreRelation entity, NpgsqlConnection connection,
        NpgsqlTransaction? transaction = null)
        => throw new NotImplementedException();

    internal override async Task<object?> ExecuteUpsert(InGenreRelation entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        var now = DateTime.Now;

        var query = UpsertBuilder.Build(
            table: "genre_relation",
            insertProperties: new Dictionary<string, object?>
            {
                { GenreRelationColumns.FromGenreId, entity.FromGenreId },
                { GenreRelationColumns.ToGenreId, entity.ToGenreId },
                { GenreRelationColumns.CreatedAt, now },
                { GenreRelationColumns.UpdatedAt, now },
                { GenreRelationColumns.Metadata, entity.Metadata },
                { GenreRelationColumns.Type, entity.Type },
                { GenreRelationColumns.Weight, entity.Weight }
            },
            conflictColumns:
            [
                GenreRelationColumns.FromGenreId,
                GenreRelationColumns.ToGenreId
            ],
            updateProperties: new Dictionary<string, object?>
            {
                { GenreRelationColumns.UpdatedAt, now },
                { GenreRelationColumns.Metadata, entity.Metadata },
                { GenreRelationColumns.Type, entity.Type },
                { GenreRelationColumns.Weight, entity.Weight }
            }
        );
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(query.Query, query.Parameters));
        
        await using var cmd = new NpgsqlCommand(query.Query, connection, transaction);
        cmd.Parameters.AddRange(query.Parameters.ToArray());
        await cmd.ExecuteNonQueryAsync();

        return null;
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