using System.Text;
using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.User;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Contracts.Dto.Responses.User;
using Musicx.Infrastructure.API.Persistence.Builders;
using Musicx.Infrastructure.API.Persistence.Columns.Song;
using Musicx.Infrastructure.API.Persistence.Columns.User;
using Musicx.Infrastructure.API.Persistence.Mappers;
using Musicx.Infrastructure.Shared.Exceptions;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Repositories.User;

internal sealed class UserSongAttrRepository(
    IDbConnectionProvider connection,
    SqlBuilder<InUserSongAttribute> builder,
    ILoggerProvider loggerProvider) : IUserSongAttrsRepository
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(UserSongAttrRepository));

    public async Task DeleteAsync(long id)
    {
        const string sql = $"DELETE FROM user_song_attrs WHERE {UserSongAttrColumns.UserId} = @id";
        await connection.ExecuteTransactionAsync((sql, [new NpgsqlParameter("@id", id)]));
    }

    public async Task DeleteAsync(long userId, long songId)
    {
        var sql = $"DELETE FROM user_song_attrs WHERE {UserSongAttrColumns.UserId} = @userId AND {UserSongAttrColumns.SongId} = @songId";
        await connection.ExecuteTransactionAsync((sql,
        [
            new NpgsqlParameter("@userId", userId),
            new NpgsqlParameter("@songId", songId)
        ]));
    }

    public async Task<OutUserSongAttribute?> FindOneBySongUserAsync(long userId, long songId)
    {
        var sql = new StringBuilder(builder.BuildSelect());
        sql.Append($" WHERE usr0.{UserSongAttrColumns.UserId} = @userId AND usr0.{UserSongAttrColumns.SongId} = @songId");

        var result = await connection.FetchListDynamicAsync(sql.ToString(),
        [
            new NpgsqlParameter("@userId", userId),
            new NpgsqlParameter("@songId", songId)
        ]);

        return result.SingleOrDefault()?.FromDicoToUserSongAttr();
    }

    public async Task<IReadOnlyList<OutUserSongAttribute>> FindByAlbumUserAsync(long userId, long albumId)
    {
        var sql = new StringBuilder(builder.BuildSelect());
        sql.Append($" WHERE usr0.{UserSongAttrColumns.UserId} = @userId AND s0.{SongColumns.AlbumId} = @albumId");

        var result = await connection.FetchListDynamicAsync(sql.ToString(),
        [
            new NpgsqlParameter("@userId", userId),
            new NpgsqlParameter("@albumId", albumId)
        ]);

        return result.Select(x => x.FromDicoToUserSongAttr()).ToList();
    }

    public async Task<long> SaveAsync(InUserSongAttribute entity)
    {
        await using var conn = (NpgsqlConnection)connection.CreateConnection();
        await conn.OpenAsync();
        await using var transaction = await conn.BeginTransactionAsync();

        try
        {
            await builder.ExecuteUpsert(entity, conn, transaction);
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new RepositoryException("❌ SAVE User Song Attr : Could not persist.", ex, _logger);
        }

        return -1;
    }
}