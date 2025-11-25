using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Infrastructure.API.Persistence.Columns;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders;

internal sealed class AlbumInfluenceSqlBuilder(ILoggerProvider loggerProvider) : SqlBuilder<InAlbumInfluence>
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(AlbumInfluenceSqlBuilder));
    
    internal override async Task<object?> ExecuteInsert(InAlbumInfluence entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        var createCommandSql = BuildInsert("album_influence",
            new Dictionary<string, object?>
            {
                { AlbumInfluenceColumns.AlbumId, entity.AlbumId },
                { AlbumInfluenceColumns.GenreId, entity.GenreId },
                { AlbumInfluenceColumns.TaggerId, entity.TaggerId },
                { AlbumInfluenceColumns.CreatedAt, DateTime.Now },
                { AlbumInfluenceColumns.UpdatedAt, DateTime.Now },
                { AlbumInfluenceColumns.Confidence, entity.Confidence },
                { AlbumInfluenceColumns.Metadata, entity.Metadata },
                { AlbumInfluenceColumns.Source, entity.Source },
            });
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(createCommandSql.Query, createCommandSql.Parameters));

        await using (var cmd = new NpgsqlCommand(createCommandSql.Query, connection, transaction))
        {
            cmd.Parameters.AddRange(createCommandSql.Parameters.ToArray());
            await cmd.ExecuteNonQueryAsync();
        }
        
        return null;
    }

    internal override async Task ExecuteUpdate(InAlbumInfluence entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        var updateCommandSql = BuildUpdate("album_influence",
            [AlbumInfluenceColumns.AlbumId, AlbumInfluenceColumns.GenreId, AlbumInfluenceColumns.TaggerId],
            [entity.AlbumId, entity.GenreId, entity.TaggerId],
            new Dictionary<string, object?>
            {
                { AlbumInfluenceColumns.AlbumId, entity.AlbumId },
                { AlbumInfluenceColumns.GenreId, entity.GenreId },
                { AlbumInfluenceColumns.TaggerId, entity.TaggerId },
                { AlbumInfluenceColumns.UpdatedAt, DateTime.Now },
                { AlbumInfluenceColumns.Confidence, entity.Confidence },
                { AlbumInfluenceColumns.Metadata, entity.Metadata },
                { AlbumInfluenceColumns.Source, entity.Source },
        });
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(updateCommandSql.Query, updateCommandSql.Parameters));

        await using var cmd = new NpgsqlCommand(updateCommandSql.Query, connection, transaction);
        cmd.Parameters.AddRange(updateCommandSql.Parameters.ToArray());
        await cmd.ExecuteScalarAsync();
    }

    internal override string BuildSelect(IQuerySpecification<InAlbumInfluence>? querySpecification = null, bool distinct = false)
        => distinct
            ? "SELECT DISTINCT ag0.*, al0.*, g0.*, u0.* FROM album_influence ag0 " + 
              $"INNER JOIN genres g0 ON ag0.{AlbumInfluenceColumns.GenreId} = g0.{GenreColumns.Id} " +
              $"INNER JOIN users u0 ON ag0.{AlbumInfluenceColumns.TaggerId} = u0.{UserColumns.Id} " +
              $"INNER JOIN albums al0 ON ag0.{AlbumInfluenceColumns.AlbumId} = al0.{AlbumColumns.Id} "
            : "SELECT ag0.*, al0.*, g0.*, u0.* FROM album_influence ag0 " + 
              $"INNER JOIN genres g0 ON ag0.{AlbumInfluenceColumns.GenreId} = g0.{GenreColumns.Id} " +
              $"INNER JOIN users u0 ON ag0.{AlbumInfluenceColumns.TaggerId} = u0.{UserColumns.Id} " +
              $"INNER JOIN albums al0 ON ag0.{AlbumInfluenceColumns.AlbumId} = al0.{AlbumColumns.Id} ";

    internal override string BuildGroupBy(IQuerySpecification<InAlbumInfluence>? querySpecification = null)
        => string.Empty;
}