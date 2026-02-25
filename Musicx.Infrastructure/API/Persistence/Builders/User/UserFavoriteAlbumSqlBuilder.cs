using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Infrastructure.API.Persistence.Columns.Album;
using Musicx.Infrastructure.API.Persistence.Columns.User;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders.User;

internal sealed class UserFavoriteAlbumSqlBuilder(
    ILoggerProvider loggerProvider) : SqlBuilder<InUserFavoriteAlbum>
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(UserFavoriteAlbumSqlBuilder));
    
    internal override Task<object?> ExecuteInsert(InUserFavoriteAlbum entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        throw new NotImplementedException();
    }

    internal override Task ExecuteUpdate(InUserFavoriteAlbum entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        throw new NotImplementedException();
    }

    internal override async Task<object?> ExecuteUpsert(InUserFavoriteAlbum entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        var now = DateTime.Now;

        var query = UpsertBuilder.Build(
            table: "user_fav_album",
            insertProperties: new Dictionary<string, object?>
            {
                { UserFavoriteAlbumColumns.AlbumId, entity.AlbumId },
                { UserFavoriteAlbumColumns.UserId, entity.UserId },
                { UserFavoriteAlbumColumns.CreatedAt, now },
                { UserFavoriteAlbumColumns.UpdatedAt, now },
                { UserFavoriteAlbumColumns.Note, entity.Note },
                { UserFavoriteAlbumColumns.Order, entity.Order }
            },
            conflictColumns:
            [
                UserFavoriteAlbumColumns.AlbumId,
                UserFavoriteAlbumColumns.UserId
            ],
            updateProperties: new Dictionary<string, object?>
            {
                { UserFavoriteAlbumColumns.UpdatedAt, now },
                { UserFavoriteAlbumColumns.Note, entity.Note },
                { UserFavoriteAlbumColumns.Order, entity.Order }
            }
        );
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(query.Query, query.Parameters));
        
        await using var cmd = new NpgsqlCommand(query.Query, connection, transaction);
        cmd.Parameters.AddRange(query.Parameters.ToArray());
        await cmd.ExecuteNonQueryAsync();

        return null;
    }

    internal override string BuildSelect(IJoinSpecification<InUserFavoriteAlbum>? spec = null, bool distinct = false)
        => distinct
            ? "SELECT DISTINCT ufal0.*, al0.*, u0.* FROM user_fav_album ufal0 " +
              $"JOIN users u0 ON ufal0.{UserFavoriteAlbumColumns.UserId} = u0.{UserColumns.Id} " +
              $"JOIN albums al0 ON ufal0.{UserFavoriteAlbumColumns.AlbumId} = al0.{AlbumColumns.Id} "
            : "SELECT ufal0.*, al0.*, u0.* FROM user_fav_album ufal0 " +
              $"JOIN users u0 ON ufal0.{UserFavoriteAlbumColumns.UserId} = u0.{UserColumns.Id} " +
              $"JOIN albums al0 ON ufal0.{UserFavoriteAlbumColumns.AlbumId} = al0.{AlbumColumns.Id} ";

    internal override string BuildGroupBy(IJoinSpecification<InUserFavoriteAlbum>? spec = null)
        => string.Empty;
}