using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Infrastructure.API.Persistence.Columns;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders;

internal sealed class GenreAliasSqlBuilder(ILoggerProvider loggerProvider) : SqlBuilder<InGenreAlias>
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(GenreAliasSqlBuilder));

    internal override async Task<object?> ExecuteInsert(InGenreAlias entity, NpgsqlConnection connection,
        NpgsqlTransaction? transaction = null)
    {
        long genreAliasId;

        var createCommandSql = BuildInsert("genre_alias",
            new Dictionary<string, object?>
            {
                { GenreAliasColumns.GenreId, entity.GenreId },
                { GenreAliasColumns.CreatedAt, DateTime.Now },
                { GenreAliasColumns.UpdatedAt, DateTime.Now },
                { GenreAliasColumns.Lang, entity.Lang },
                { GenreAliasColumns.Metadata, entity.Metadata },
                { GenreAliasColumns.Name, entity.Name },
            },
            returningColumn: GenreAliasColumns.Id);
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(createCommandSql.Query, createCommandSql.Parameters));

        await using (var cmd = new NpgsqlCommand(createCommandSql.Query, connection, transaction))
        {
            cmd.Parameters.AddRange(createCommandSql.Parameters.ToArray());
            genreAliasId = (long) (await cmd.ExecuteScalarAsync() ??
                                  throw new NullReferenceException("Could not insert entity"));
        }
        
        _logger.LogDebug("ℹ️ Id for new entity is : " + genreAliasId);

        return genreAliasId;
    }

    internal override async Task ExecuteUpdate(InGenreAlias entity, NpgsqlConnection connection,
        NpgsqlTransaction? transaction = null)
    {
        var updateCommandSql = BuildUpdate("genre_alias",
            GenreAliasColumns.Id,
            entity.Id,
            new Dictionary<string, object?>
            {
                { GenreAliasColumns.UpdatedAt, DateTime.Now },
                { GenreAliasColumns.Lang, entity.Lang },
                { GenreAliasColumns.Metadata, entity.Metadata },
                { GenreAliasColumns.Name, entity.Name },
            });
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(updateCommandSql.Query, updateCommandSql.Parameters));

        await using (var cmd = new NpgsqlCommand(updateCommandSql.Query, connection, transaction))
        {
            cmd.Parameters.AddRange(updateCommandSql.Parameters.ToArray());
            await cmd.ExecuteNonQueryAsync();
        }
    }

    internal override Task ExecuteUpsert(InGenreAlias entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        throw new NotImplementedException();
    }

    internal override string BuildSelect(IQuerySpecification<InGenreAlias>? querySpecification = null,
        bool distinct = false)
        => distinct
            ? "SELECT DISTINCT ga0.* FROM genre_alias ga0 " +
              $"JOIN genres g0 ON ga0.{GenreAliasColumns.GenreId} = g0.{GenreColumns.Id} "
            : "SELECT ga0.* FROM genre_alias ga0 " +
              $"JOIN genres g0 ON ga0.{GenreAliasColumns.GenreId} = g0.{GenreColumns.Id} ";

    internal override string BuildGroupBy(IQuerySpecification<InGenreAlias>? spec = null)
        => $" GROUP BY ga0.{GenreAliasColumns.Id}";
}