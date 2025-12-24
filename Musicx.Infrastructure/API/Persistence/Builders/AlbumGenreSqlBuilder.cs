using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Specifics;
using Musicx.Contracts.Enums;
using Musicx.Infrastructure.API.Persistence.Columns;
using Musicx.Infrastructure.API.Persistence.Helpers;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders;

internal sealed class AlbumGenreSqlBuilder(ILoggerProvider loggerProvider) : SqlBuilder<InAlbumGenre>
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(AlbumGenreSqlBuilder));
    
    internal override async Task<object?> ExecuteInsert(InAlbumGenre entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        var createCommandSql = BuildInsert("album_genre",
            new Dictionary<string, object?>
            {
                { AlbumGenreColumns.AlbumId, entity.AlbumId },
                { AlbumGenreColumns.GenreId, entity.GenreId },
                { AlbumGenreColumns.TaggerId, entity.TaggerId },
                { AlbumGenreColumns.CreatedAt, DateTime.Now },
                { AlbumGenreColumns.UpdatedAt, DateTime.Now },
                { AlbumGenreColumns.Confidence, entity.Confidence },
                { AlbumGenreColumns.Metadata, entity.Metadata },
                { AlbumGenreColumns.Source, entity.Source },
            });
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(createCommandSql.Query, createCommandSql.Parameters));

        await using (var cmd = new NpgsqlCommand(createCommandSql.Query, connection, transaction))
        {
            cmd.Parameters.AddRange(createCommandSql.Parameters.ToArray());
            await cmd.ExecuteNonQueryAsync();
        }
        
        return null;
    }

    internal override async Task ExecuteUpdate(InAlbumGenre entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        var updateCommandSql = BuildUpdate("album_genre",
            [AlbumGenreColumns.AlbumId, AlbumGenreColumns.GenreId, AlbumGenreColumns.TaggerId],
            [entity.AlbumId, entity.GenreId, entity.TaggerId],
            new Dictionary<string, object?>
            {
                { AlbumGenreColumns.UpdatedAt, DateTime.Now },
                { AlbumGenreColumns.Confidence, entity.Confidence },
                { AlbumGenreColumns.Metadata, entity.Metadata },
                { AlbumGenreColumns.Source, entity.Source },
        });
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(updateCommandSql.Query, updateCommandSql.Parameters));

        await using var cmd = new NpgsqlCommand(updateCommandSql.Query, connection, transaction);
        cmd.Parameters.AddRange(updateCommandSql.Parameters.ToArray());
        await cmd.ExecuteScalarAsync();
    }

    internal override string BuildSelect(IQuerySpecification<InAlbumGenre>? querySpecification = null, bool distinct = false)
        => distinct
            ? "SELECT DISTINCT ag0.*, al0.*, g0.*, u0.* FROM album_genre ag0 " + 
              $"INNER JOIN genres g0 ON ag0.{AlbumGenreColumns.GenreId} = g0.{GenreColumns.Id} " +
              $"INNER JOIN users u0 ON ag0.{AlbumGenreColumns.TaggerId} = u0.{UserColumns.Id} " +
              $"INNER JOIN albums al0 ON ag0.{AlbumGenreColumns.AlbumId} = al0.{AlbumColumns.Id} "
            : "SELECT ag0.*, al0.*, g0.*, u0.* FROM album_genre ag0 " + 
              $"INNER JOIN genres g0 ON ag0.{AlbumGenreColumns.GenreId} = g0.{GenreColumns.Id} " +
              $"INNER JOIN users u0 ON ag0.{AlbumGenreColumns.TaggerId} = u0.{UserColumns.Id} " +
              $"INNER JOIN albums al0 ON ag0.{AlbumGenreColumns.AlbumId} = al0.{AlbumColumns.Id} ";

    internal override string BuildGroupBy(IQuerySpecification<InAlbumGenre>? querySpecification = null)
        => string.Empty;
}