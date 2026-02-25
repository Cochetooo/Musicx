using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Infrastructure.API.Persistence.Columns.Album;
using Musicx.Infrastructure.API.Persistence.Columns.Artist;
using Musicx.Infrastructure.API.Persistence.Columns.User;
using Musicx.Infrastructure.API.Persistence.Specifications.User;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders.User;

internal sealed class UserAlbumAttrSqlBuilder(
    ILoggerProvider loggerProvider) : SqlBuilder<InUserAlbumAttribute>
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(UserAlbumAttrSqlBuilder));
    
    internal override async Task<object?> ExecuteInsert(InUserAlbumAttribute entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        var createCommandSql = InsertBuilder.Build("user_album_attrs",
            new Dictionary<string, object?>
            {
                { UserAlbumAttrColumns.AlbumId, entity.AlbumId },
                { UserAlbumAttrColumns.UserId, entity.UserId },
                { UserAlbumAttrColumns.CreatedAt, DateTime.Now },
                { UserAlbumAttrColumns.UpdatedAt, DateTime.Now },
                { UserAlbumAttrColumns.CollectionType, entity.CollectionType },
                { UserAlbumAttrColumns.DiscoveryDate, entity.DiscoveryDate },
                { UserAlbumAttrColumns.Rating, entity.Rating },
                { UserAlbumAttrColumns.Review, entity.Review },
            });
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(createCommandSql.Query, createCommandSql.Parameters));
        
        await using (var cmd = new NpgsqlCommand(createCommandSql.Query, connection, transaction))
        {
            cmd.Parameters.AddRange(createCommandSql.Parameters.ToArray());
            await cmd.ExecuteNonQueryAsync();
        }

        return null;
    }

    internal override async Task ExecuteUpdate(InUserAlbumAttribute entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        var createCommandSql = UpdateBuilder.Build("user_album_attrs",
            new Dictionary<string, object?>
            {
                { UserAlbumAttrColumns.UpdatedAt, DateTime.Now },
                { UserAlbumAttrColumns.CollectionType, entity.CollectionType },
                { UserAlbumAttrColumns.DiscoveryDate, entity.DiscoveryDate },
                { UserAlbumAttrColumns.Rating, entity.Rating },
                { UserAlbumAttrColumns.Review, entity.Review },
            },
            new Dictionary<string, object?>
            {
                { UserAlbumAttrColumns.UserId, entity.UserId },
                { UserAlbumAttrColumns.AlbumId, entity.AlbumId },
            }
        );
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(createCommandSql.Query, createCommandSql.Parameters));
        
        await using (var cmd = new NpgsqlCommand(createCommandSql.Query, connection, transaction))
        {
            cmd.Parameters.AddRange(createCommandSql.Parameters.ToArray());
            await cmd.ExecuteNonQueryAsync();
        }
    }

    internal override Task ExecuteUpsert(InUserAlbumAttribute entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        throw new NotImplementedException();
    }

    internal override string BuildSelect(IJoinSpecification<InUserAlbumAttribute>? spec = null, bool distinct = false)
    {
        if (spec is not UserAlbumAttrJoinSpecification specUserAlbumAttr)
        {
            return distinct
                ? "SELECT DISTINCT uaa0.*, u0.*, al0.* FROM user_album_attrs uaa0 " +
                  $"JOIN users u0 ON uaa0.{UserAlbumAttrColumns.UserId} = u0.{UserColumns.Id} " +
                  $"JOIN albums al0 ON uaa0.{UserAlbumAttrColumns.AlbumId} = al0.{AlbumColumns.Id} "
                : "SELECT uaa0.*, u0.*, al0.* FROM user_album_attrs uaa0 " +
                  $"JOIN users u0 ON uaa0.{UserAlbumAttrColumns.UserId} = u0.{UserColumns.Id} " +
                  $"JOIN albums al0 ON uaa0.{UserAlbumAttrColumns.AlbumId} = al0.{AlbumColumns.Id} ";
        }

        var selects = new List<string> { "uaa0.*", "u0.*", "al0.*" };
        var joins = new List<string>
        {
            $"JOIN users u0 ON uaa0.{UserAlbumAttrColumns.UserId} = u0.{UserColumns.Id}",
            $"JOIN albums al0 ON uaa0.{UserAlbumAttrColumns.AlbumId} = al0.{AlbumColumns.Id}"
        };

        if (specUserAlbumAttr.IncludeAlbumArtists)
        {
            selects.Add("ar0.*");
            joins.Add($"LEFT JOIN artists ar0 ON al0.{AlbumColumns.ArtistId} = ar0.{ArtistColumns.Id}");
        }
        
        return distinct
            ? $"SELECT DISTINCT {string.Join(", ", selects)} FROM user_album_attrs uaa0 {string.Join(" ", joins)}"
            : $"SELECT {string.Join(", ", selects)} FROM user_album_attrs uaa0 {string.Join(" ", joins)}";
    }

    internal override string BuildGroupBy(IJoinSpecification<InUserAlbumAttribute>? spec = null)
        => string.Empty;
}