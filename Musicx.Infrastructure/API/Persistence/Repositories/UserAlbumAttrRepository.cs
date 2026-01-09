using System.Text;
using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.User;
using Musicx.Application.Api.Interfaces.Specifications;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Ratings;
using Musicx.Infrastructure.API.Persistence.Builders;
using Musicx.Infrastructure.API.Persistence.Columns;
using Musicx.Infrastructure.API.Persistence.Connection;
using Musicx.Infrastructure.API.Persistence.Mappers;
using Musicx.Infrastructure.Shared.Exceptions;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Repositories;

internal sealed class UserAlbumAttrRepository(
    IDbConnectionProvider connection,
    SqlBuilder<InUserAlbumAttribute> builder,
    ILoggerProvider loggerProvider) : IUserAlbumAttrsRepository 
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(UserAlbumAttrRepository));
    
    public async Task DeleteAsync(long id)
    {
        const string sql = $"DELETE FROM user_album_attrs WHERE {UserAlbumAttrColumns.UserId} = @id";

        var parameters = new List<NpgsqlParameter>
        {
            new("@id", id)
        };

        await connection.ExecuteTransactionAsync((sql, parameters));
    }
    
    public async Task DeleteAsync(long userId, long albumId)
    {
        const string sql = $"DELETE FROM user_album_attrs " +
                           $"WHERE {UserAlbumAttrColumns.UserId} = @userId AND {UserAlbumAttrColumns.AlbumId} = @albumId";

        var parameters = new List<NpgsqlParameter>
        {
            new("@userId", userId),
            new("@albumId", albumId)
        };

        await connection.ExecuteTransactionAsync((sql, parameters));
    }

    public Task DeleteAllAsync(IEnumerable<long> ids)
        => throw new NotImplementedException("DeleteAllAsync is disabled on this repository.");

    public Task<OutUserAlbumAttribute?> FindByIdAsync(long id, IQuerySpecification<InUserAlbumAttribute>? songQuerySpecification = null)
        => throw new NotImplementedException("FindByIdAsync is disabled on this repository.");

    public Task<List<OutUserAlbumAttribute>> FindAsync(long skip = 0, long take = 100, 
        bool? filterExact = null, double? filterSimilitude = 0.4, string? filter = null,
        string? order = null, IQuerySpecification<InUserAlbumAttribute>? songQuerySpecification = null)
        => throw new NotImplementedException("FindAsync is disabled on this repository.");

    public Task<List<OutUserAlbumAttribute>> FindIn(IEnumerable<long> ids, IQuerySpecification<InUserAlbumAttribute>? songQuerySpecification = null)
        => throw new NotImplementedException("FindIn is disabled on this repository.");

    public Task<long> GetCountAsync()
        => throw new NotImplementedException("GetCountAsync is disabled on this repository.");

    public async Task<long> SaveAsync(InUserAlbumAttribute entity)
    {
        await using var conn = connection.CreateConnection();
        await conn.OpenAsync();
        
        await using var transaction = await conn.BeginTransactionAsync();
        
        try
        {
            var exist = await FindOneAlbumFromUserAsync(entity.UserId, entity.AlbumId);
            
            if (exist is null)
            {
                await builder.ExecuteInsert(entity, conn, transaction);
            }
            else
            {
                await builder.ExecuteUpdate(entity, conn, transaction);
            }
            
            await transaction.CommitAsync();
        } 
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new RepositoryException("❌ SAVE Album : Could not persist.", ex, _logger);
        }
        
        return -1;
    }

    public async Task<List<long>> SaveAllAsync(IEnumerable<InUserAlbumAttribute> entities)
        => throw new NotImplementedException();

    public async Task<long> CountByAlbumIdAsync(long albumId)
    {
        await using var conn = connection.CreateConnection();
        await conn.OpenAsync();
        
        var sql = $"SELECT COUNT(*) FROM user_album_attrs WHERE {UserAlbumAttrColumns.AlbumId} = @albumId";
        
        await using var command = new NpgsqlCommand(sql, conn);
        var parameters = new List<NpgsqlParameter>()
        {
            new("@albumId", albumId)
        };
        
        command.Parameters.AddRange(parameters.ToArray());
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(sql, parameters));

        try
        {
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt64(await command.ExecuteScalarAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError("❌ Could not execute cound by album command for table user_album_attrs.");
            return -1;
        }
    }

    public async Task<long> CountByUserIdAsync(long userId, 
        IQuerySpecification<InUserAlbumAttribute>? spec, long? artistId = null,
        bool? filterExact = null, double? filterSimilitude = 0.4, string? filter = null)
    {
        await using var conn = connection.CreateConnection();
        await conn.OpenAsync();
        
        var parameters = new List<NpgsqlParameter>()
        {
            new("@userId", userId)
        };
        
        var sql = $"SELECT COUNT(*) FROM user_album_attrs uaa0 "
               + $"JOIN albums al0 ON uaa0.{UserAlbumAttrColumns.AlbumId} = al0.{AlbumColumns.Id} ";

        if (spec is UserAlbumAttrSpecification uaaSpec)
        {
            if (uaaSpec.IncludeAlbumArtists)
            {
                sql += $"JOIN artists ar0 ON al0.{AlbumColumns.ArtistId} = ar0.{ArtistColumns.Id} ";
            }
        }

        if (!string.IsNullOrWhiteSpace(filter))
        {
            builder.Filter(
                sql: ref sql, 
                columns: [$"al0.{AlbumColumns.Name}", $"ar0.{ArtistColumns.Name}"], 
                filter: filter,
                parameters: parameters, 
                filterExact: filterExact, 
                filterSimilitude: filterSimilitude
            );
            sql += $" AND {UserAlbumAttrColumns.UserId} = @userId";
        }
        else
        {
            sql += $" WHERE {UserAlbumAttrColumns.UserId} = @userId";
        }
        
        await using var command = new NpgsqlCommand(sql, conn);
        
        command.Parameters.AddRange(parameters.ToArray());
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(sql, parameters));

        try
        {
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(await command.ExecuteScalarAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError("❌ Could not execute count by user command for table user_album_attrs.");
            return -1;
        }
    }

    public async Task<IReadOnlyList<OutUserAlbumAttribute>> FindByAlbumIdAsync(long albumId, 
        long skip = 0, long take = 100, string? order = null)
    {
        var sql = builder.BuildSelect();
        var parameters = new List<NpgsqlParameter>();

        sql += $" WHERE {UserAlbumAttrColumns.AlbumId} = @albumId";
        parameters.Add(new NpgsqlParameter("@albumId", albumId));

        sql += builder.BuildOrderBy($"uaa0.{UserAlbumAttrColumns.UpdatedAt} DESC", $"u0.{UserColumns.Name}");
        sql += " OFFSET @skip LIMIT @take";
        
        parameters.Add(new NpgsqlParameter("@skip", skip));
        parameters.Add(new NpgsqlParameter("@take", take));

        var result = await connection.FetchListDynamicAsync(sql, parameters);

        return result
            .Select(x => x.FromDicoToUserAlbumAttr())
            .ToList();
    }

    public async Task<IReadOnlyList<OutUserAlbumAttribute>> FindByUserIdAsync(long userId, 
        IQuerySpecification<InUserAlbumAttribute>? spec = null,
        long skip = 0, long take = 100, long? artistId = null, 
        bool? filterExact = null, double? filterSimilitude = 0.4,
        string? filter = null, string? order = null)
    {
        var sql = builder.BuildSelect(spec);
        var parameters = new List<NpgsqlParameter>();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            builder.Filter(
                sql: ref sql, 
                columns: [$"al0.{AlbumColumns.Name}", $"ar0.{ArtistColumns.Name}"], 
                filter: filter,
                parameters: parameters, 
                filterExact: filterExact, 
                filterSimilitude: filterSimilitude
            );
            sql += $" AND {UserAlbumAttrColumns.UserId} = @userId";
        }
        else
        {
            sql += $" WHERE {UserAlbumAttrColumns.UserId} = @userId";
        }
        
        parameters.Add(new NpgsqlParameter("@userId", userId));

        if (artistId is not null)
        {
            sql += $" AND al0.{AlbumColumns.ArtistId} = @artistId";
            parameters.Add(new NpgsqlParameter("@artistId", artistId));
        }

        sql += builder.BuildOrderBy($"uaa0.{UserAlbumAttrColumns.UpdatedAt} DESC", $"u0.{UserColumns.Name}");
        sql += " OFFSET @skip LIMIT @take";
        
        parameters.Add(new NpgsqlParameter("@skip", skip));
        parameters.Add(new NpgsqlParameter("@take", take));

        var result = await connection.FetchListDynamicAsync(sql, parameters);

        return result
            .Select(x => x.FromDicoToUserAlbumAttr())
            .ToList();
    }

    public async Task<OutUserAlbumAttribute?> FindOneAlbumFromUserAsync(long userId, long albumId)
    {
        var sql = new StringBuilder(builder.BuildSelect());
        sql.Append($" WHERE {UserAlbumAttrColumns.UserId} = @userId AND {UserAlbumAttrColumns.AlbumId} = @albumId");

        var parameters = new List<NpgsqlParameter>
        {
            new("@userId", userId),
            new("@albumId", albumId)
        };

        var result = await connection.FetchListDynamicAsync(sql.ToString(), parameters);

        return result
            .SingleOrDefault()?
            .FromDicoToUserAlbumAttr();
    }

    public async Task<OutUserRatingStats> GetUserRatingStatsAsync(long userId)
    {
        var sql = """
                  SELECT 
                      (user_album_attrs_rating / 10) * 10 AS range_floor,
                      COUNT(*) AS count
                  FROM user_album_attrs
                  WHERE user_album_attrs_user_id = @userId
                  GROUP BY (user_album_attrs_rating / 10) * 10
                  ORDER BY range_floor;
                  """;
        
        var parameters = new List<NpgsqlParameter>()
        {
            new("@userId", userId)
        };

        var result = await connection.FetchListDynamicAsync(sql, parameters);

        var ratingCounts = result
            .Select(x =>
            {
                var dict = (IDictionary<string, object>)x;
                var key = Convert.ToInt32(dict["range_floor"]);
                var value = Convert.ToInt32(dict["count"]);
                return new KeyValuePair<int, int>(key, value);
            })
            .ToDictionary(kv => kv.Key, kv => kv.Value);

        return new OutUserRatingStats
        {
            UserId = userId,
            RatingCounts = ratingCounts
        };
    }
}