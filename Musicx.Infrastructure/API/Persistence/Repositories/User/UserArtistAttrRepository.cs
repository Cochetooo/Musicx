using System.Text;
using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.User;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Contracts.Dto.Responses;
using Musicx.Infrastructure.API.Persistence.Builders;
using Musicx.Infrastructure.API.Persistence.Columns.Artist;
using Musicx.Infrastructure.API.Persistence.Columns.User;
using Musicx.Infrastructure.API.Persistence.Mappers;
using Musicx.Infrastructure.Shared.Exceptions;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Repositories.User;

internal sealed class UserArtistAttrRepository(
    IDbConnectionProvider connection,
    SqlBuilder<InUserArtistAttribute> builder,
    ILoggerProvider loggerProvider) : IUserArtistAttrsRepository
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(UserArtistAttrRepository));
    
    public async Task DeleteAsync(long userId, long artistId)
    {
        const string sql = $"DELETE FROM user_artist_attrs WHERE {UserArtistAttrColumns.UserId} = @userId AND {UserArtistAttrColumns.ArtistId} = @artistId";
        await connection.ExecuteTransactionAsync((sql, new List<NpgsqlParameter>
        {
            new("@userId", userId),
            new("@artistId", artistId)
        }));
    }
    
    public async Task<IReadOnlyList<OutUserArtistAttribute>> FindByArtistAsync(long artistId, bool followersOnly = true)
    {
        var sql = new StringBuilder(builder.BuildSelect());
        sql.Append($" WHERE uar0.{UserArtistAttrColumns.ArtistId} = @artistId");
        if (followersOnly)
        {
            sql.Append($" AND COALESCE(uar0.{UserArtistAttrColumns.Follow}, false) = true");
        }
        sql.Append($" ORDER BY u0.{UserColumns.Name}");

        var result = await connection.FetchListDynamicAsync(sql.ToString(), [new NpgsqlParameter("@artistId", artistId)]);
        return result.Select(x => x.FromDicoToUserArtistAttr()).ToList();
    }
    
    public async Task<IReadOnlyList<OutUserArtistAttribute>> FindByUserAsync(long userId, bool followersOnly = true)
    {
        var sql = new StringBuilder(builder.BuildSelect());
        sql.Append($" WHERE uar0.{UserArtistAttrColumns.UserId} = @userId");
        if (followersOnly)
        {
            sql.Append($" AND COALESCE(uar0.{UserArtistAttrColumns.Follow}, false) = true");
        }
        sql.Append($" ORDER BY ar0.{ArtistColumns.Name}");

        var result = await connection.FetchListDynamicAsync(sql.ToString(), [new NpgsqlParameter("@userId", userId)]);
        return result.Select(x => x.FromDicoToUserArtistAttr()).ToList();
    }
    
    public async Task<OutUserArtistAttribute?> FindOneAsync(long userId, long artistId)
    {
        var sql = new StringBuilder(builder.BuildSelect());
        sql.Append($" WHERE uar0.{UserArtistAttrColumns.UserId} = @userId AND uar0.{UserArtistAttrColumns.ArtistId} = @artistId");

        var result = await connection.FetchListDynamicAsync(sql.ToString(),
        [
            new NpgsqlParameter("@userId", userId),
            new NpgsqlParameter("@artistId", artistId)
        ]);

        return result.SingleOrDefault()?.FromDicoToUserArtistAttr();
    }
    
    public async Task<long> CountFollowersByArtistAsync(long artistId)
    {
        const string sql = $"SELECT COUNT(*) FROM user_artist_attrs WHERE {UserArtistAttrColumns.ArtistId} = @artistId AND COALESCE({UserArtistAttrColumns.Follow}, false) = true";
        await using var conn = (NpgsqlConnection)connection.CreateConnection();
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.Add(new NpgsqlParameter("@artistId", artistId));
        var result = await cmd.ExecuteScalarAsync();
        return result is null ? 0 : Convert.ToInt64(result);
    }
    
    public async Task<long> SaveAsync(InUserArtistAttribute entity)
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
            throw new RepositoryException("❌ SAVE User Artist Attr : Could not persist.", ex, _logger);
        }

        return -1;
    }
    
    public async Task<List<long>> SaveAllAsync(IEnumerable<InUserArtistAttribute> entities)
    {
        await using var conn = (NpgsqlConnection)connection.CreateConnection();
        await conn.OpenAsync();
        await using var transaction = await conn.BeginTransactionAsync();

        try
        {
            foreach (var entity in entities)
            {
                await builder.ExecuteUpsert(entity, conn, transaction);
            }

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new RepositoryException("❌ SAVE ALL User Artist Attr : Could not persist.", ex, _logger);
        }

        return [];
    }
}