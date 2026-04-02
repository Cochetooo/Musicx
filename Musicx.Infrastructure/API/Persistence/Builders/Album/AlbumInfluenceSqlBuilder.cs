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

internal sealed class AlbumInfluenceSqlBuilder(ILoggerProvider loggerProvider) : SqlBuilder<InAlbumInfluence>
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(AlbumInfluenceSqlBuilder));
    
    internal override async Task<object?> ExecuteUpsert(InAlbumInfluence entity, NpgsqlConnection connection,
        NpgsqlTransaction? transaction = null)
    {
        var now = DateTime.Now;

        var query = UpsertBuilder.Build(
            table: "album_influence",
            insertProperties: new Dictionary<string, object?>
            {
                { AlbumInfluenceColumns.AlbumId, entity.AlbumId },
                { AlbumInfluenceColumns.GenreId, entity.GenreId },
                { AlbumInfluenceColumns.TaggerId, entity.TaggerId },
                { AlbumInfluenceColumns.CreatedAt, DateTime.Now },
                { AlbumInfluenceColumns.UpdatedAt, DateTime.Now },
                { AlbumInfluenceColumns.Confidence, entity.Confidence },
                { AlbumInfluenceColumns.Metadata, entity.Metadata },
                { AlbumInfluenceColumns.Source, entity.Source },
            },
            conflictColumns:
            [
                AlbumInfluenceColumns.AlbumId,
                AlbumInfluenceColumns.GenreId,
                AlbumInfluenceColumns.TaggerId
            ],
            updateProperties: new Dictionary<string, object?>
            {
                { AlbumInfluenceColumns.UpdatedAt, now },
                { AlbumInfluenceColumns.Confidence, entity.Confidence },
                { AlbumInfluenceColumns.Metadata, entity.Metadata },
                { AlbumInfluenceColumns.Source, entity.Source },
            }
        );
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(query.Query, query.Parameters));
        
        await using var cmd = new NpgsqlCommand(query.Query, connection, transaction);
        cmd.Parameters.AddRange(query.Parameters.ToArray());
        await cmd.ExecuteNonQueryAsync();

        return null;
    }
    
    internal override Task<object?> ExecuteInsert(InAlbumInfluence entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
        => throw new NotImplementedException();

    internal override Task ExecuteUpdate(InAlbumInfluence entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
        => throw new NotImplementedException();

    internal override string BuildSelect(IJoinSpecification<InAlbumInfluence>? querySpecification = null, bool distinct = false)
        => distinct
            ? "SELECT DISTINCT ag0.*, al0.*, g0.*, u0.* FROM album_influence ag0 " + 
              $"INNER JOIN genres g0 ON ag0.{AlbumInfluenceColumns.GenreId} = g0.{GenreColumns.Id} " +
              $"INNER JOIN users u0 ON ag0.{AlbumInfluenceColumns.TaggerId} = u0.{UserColumns.Id} " +
              $"INNER JOIN albums al0 ON ag0.{AlbumInfluenceColumns.AlbumId} = al0.{AlbumColumns.Id} "
            : "SELECT ag0.*, al0.*, g0.*, u0.* FROM album_influence ag0 " + 
              $"INNER JOIN genres g0 ON ag0.{AlbumInfluenceColumns.GenreId} = g0.{GenreColumns.Id} " +
              $"INNER JOIN users u0 ON ag0.{AlbumInfluenceColumns.TaggerId} = u0.{UserColumns.Id} " +
              $"INNER JOIN albums al0 ON ag0.{AlbumInfluenceColumns.AlbumId} = al0.{AlbumColumns.Id} ";

    internal override string BuildGroupBy(IJoinSpecification<InAlbumInfluence>? querySpecification = null)
        => string.Empty;
}