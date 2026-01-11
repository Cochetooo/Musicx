using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Album;
using Musicx.Infrastructure.API.Persistence.Columns.Album;
using Musicx.Infrastructure.API.Persistence.Columns.Genre;
using Musicx.Infrastructure.API.Persistence.Columns.User;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders.Album;

internal sealed class AlbumGenreSqlBuilder(ILoggerProvider loggerProvider) : SqlBuilder<InAlbumGenre>
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(AlbumGenreSqlBuilder));

    internal override async Task<object?> ExecuteUpsert(InAlbumGenre entity, NpgsqlConnection connection,
        NpgsqlTransaction? transaction = null)
    {
        var now = DateTime.Now;

        var query = BuildUpsert(
            table: "album_genre",
            insertProperties: new Dictionary<string, object?>
            {
                { AlbumGenreColumns.AlbumId, entity.AlbumId },
                { AlbumGenreColumns.GenreId, entity.GenreId },
                { AlbumGenreColumns.TaggerId, entity.TaggerId },
                { AlbumGenreColumns.CreatedAt, now },
                { AlbumGenreColumns.UpdatedAt, now },
                { AlbumGenreColumns.Confidence, entity.Confidence },
                { AlbumGenreColumns.Metadata, entity.Metadata },
                { AlbumGenreColumns.Source, entity.Source },
            },
            conflictColumns:
            [
                AlbumGenreColumns.AlbumId,
                AlbumGenreColumns.GenreId,
                AlbumGenreColumns.TaggerId
            ],
            updateProperties: new Dictionary<string, object?>
            {
                { AlbumGenreColumns.UpdatedAt, now },
                { AlbumGenreColumns.Confidence, entity.Confidence },
                { AlbumGenreColumns.Metadata, entity.Metadata },
                { AlbumGenreColumns.Source, entity.Source },
            }
        );
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(query.Query, query.Parameters));
        
        await using var cmd = new NpgsqlCommand(query.Query, connection, transaction);
        cmd.Parameters.AddRange(query.Parameters.ToArray());
        await cmd.ExecuteNonQueryAsync();

        return null;
    }
    
    internal override async Task<object?> ExecuteInsert(InAlbumGenre entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
        => throw new NotImplementedException();

    internal override async Task ExecuteUpdate(InAlbumGenre entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
        => throw new NotImplementedException();

    internal override string BuildSelect(IJoinSpecification<InAlbumGenre>? querySpecification = null, bool distinct = false)
        => distinct
            ? "SELECT DISTINCT ag0.*, al0.*, g0.*, u0.* FROM album_genre ag0 " + 
              $"INNER JOIN genres g0 ON ag0.{AlbumGenreColumns.GenreId} = g0.{GenreColumns.Id} " +
              $"INNER JOIN users u0 ON ag0.{AlbumGenreColumns.TaggerId} = u0.{UserColumns.Id} " +
              $"INNER JOIN albums al0 ON ag0.{AlbumGenreColumns.AlbumId} = al0.{AlbumColumns.Id} "
            : "SELECT ag0.*, al0.*, g0.*, u0.* FROM album_genre ag0 " + 
              $"INNER JOIN genres g0 ON ag0.{AlbumGenreColumns.GenreId} = g0.{GenreColumns.Id} " +
              $"INNER JOIN users u0 ON ag0.{AlbumGenreColumns.TaggerId} = u0.{UserColumns.Id} " +
              $"INNER JOIN albums al0 ON ag0.{AlbumGenreColumns.AlbumId} = al0.{AlbumColumns.Id} ";

    internal override string BuildGroupBy(IJoinSpecification<InAlbumGenre>? querySpecification = null)
        => string.Empty;
}