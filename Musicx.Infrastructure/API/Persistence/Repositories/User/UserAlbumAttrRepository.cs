using System.Text;
using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.User;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Contracts.Dto.Responses.Specifics.Ratings;
using Musicx.Infrastructure.API.Persistence.Builders;
using Musicx.Infrastructure.API.Persistence.Columns.Album;
using Musicx.Infrastructure.API.Persistence.Columns.Artist;
using Musicx.Infrastructure.API.Persistence.Columns.User;
using Musicx.Infrastructure.API.Persistence.Connection;
using Musicx.Infrastructure.API.Persistence.Mappers;
using Musicx.Infrastructure.API.Persistence.Specifications.User;
using Musicx.Infrastructure.Shared.Exceptions;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Repositories.User;

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

    public async Task<long> CountByAlbumIdAsync(long albumId)
    {
        await using var conn = (NpgsqlConnection)connection.CreateConnection();
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
        IJoinSpecification<InUserAlbumAttribute>? spec, long? artistId = null,
        bool? filterExact = null, double? filterSimilitude = 0.4, string? filter = null)
    {
        await using var conn = (NpgsqlConnection)connection.CreateConnection();
        await conn.OpenAsync();
        
        var parameters = new List<NpgsqlParameter>()
        {
            new("@userId", userId)
        };
        
        var sql = $"SELECT COUNT(*) FROM user_album_attrs uaa0 "
               + $"JOIN albums al0 ON uaa0.{UserAlbumAttrColumns.AlbumId} = al0.{AlbumColumns.Id} ";

        if (spec is UserAlbumAttrJoinSpecification uaaSpec)
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
        OrderSpecification<InUserAlbumAttribute>? orderSpec = null,
        PagingOptions? pagingOptions = null)
    {
        var sql = builder.BuildSelect();
        var parameters = new List<NpgsqlParameter>();

        sql += $" WHERE {UserAlbumAttrColumns.AlbumId} = @albumId";
        parameters.Add(new NpgsqlParameter("@albumId", albumId));

        if (orderSpec is not null)
        {
            sql += builder.BuildOrderBy(orderSpec);
        }
        else
        {
            sql += $" ORDER BY uaa0.{UserAlbumAttrColumns.UpdatedAt} DESC, u0.{UserColumns.Name}";
        }
        
        sql += " OFFSET @skip LIMIT @take";
        
        parameters.Add(new NpgsqlParameter("@skip", pagingOptions?.Skip ?? 0));
        parameters.Add(new NpgsqlParameter("@take", pagingOptions?.Take ?? 200));

        var result = await connection.FetchListDynamicAsync(sql, parameters);

        return result
            .Select(x => x.FromDicoToUserAlbumAttr())
            .ToList();
    }

    public async Task<IReadOnlyList<OutUserAlbumAttribute>> FindByUserIdAsync(long userId, 
        long? artistId = null, 
        bool? filterExact = null, double? filterSimilitude = 0.4, string? filter = null,
        IJoinSpecification<InUserAlbumAttribute>? joinSpec = null,
        OrderSpecification<InUserAlbumAttribute>? orderSpec = null,
        PagingOptions? pagingOptions = null)
    {
        var sql = builder.BuildSelect(joinSpec);
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

        if (orderSpec is not null)
        {
            sql += builder.BuildOrderBy(orderSpec);
        }
        else
        {
            sql += $" ORDER BY uaa0.{UserAlbumAttrColumns.UpdatedAt} DESC, u0.{UserColumns.Name}";
        }
        
        sql += " OFFSET @skip LIMIT @take";
        
        parameters.Add(new NpgsqlParameter("@skip", pagingOptions?.Skip ?? 0));
        parameters.Add(new NpgsqlParameter("@take", pagingOptions?.Take ?? 200));

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
    
    public async Task<long> SaveAsync(InUserAlbumAttribute entity)
    {
        await using var conn = (NpgsqlConnection)connection.CreateConnection();
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

    public async Task<OutUserRatingStats> GetUserRatingStatsAsync(long userId)
    {
        var sql = """
                  WITH base AS (
                      SELECT 
                          u.user_album_attrs_rating AS rating,
                          a.album_original_release_date,
                          ar.artist_name
                      FROM user_album_attrs u
                      JOIN albums a 
                          ON u.user_album_attrs_album_id = a.album_id
                      JOIN artists ar
                          ON a.album_artist_id = ar.artist_id
                      WHERE u.user_album_attrs_user_id = @userId
                  ),
                  
                  rating_buckets AS (
                      SELECT 
                          (rating / 1000) * 10 AS range_floor,
                          COUNT(*) AS count
                      FROM base
                      GROUP BY (rating / 1000) * 10
                  ),
                  
                  stats AS (
                      SELECT
                          AVG(rating)::float AS avg_rating,
                          STDDEV_SAMP(rating)::float AS stddev,
                          COUNT(*) FILTER (WHERE UPPER(LEFT(artist_name, 1)) BETWEEN 'A' AND 'M')::float
                              /
                          NULLIF(COUNT(*), 0)
                          AS am_nz_ratio
                      FROM base
                  ),
                  
                  most_rated_year AS (
                      SELECT 
                          EXTRACT(YEAR FROM album_original_release_date)::int AS release_year,
                          COUNT(*) AS count
                      FROM base
                      GROUP BY release_year
                      ORDER BY count DESC
                      LIMIT 1
                  )
                  
                  SELECT 
                      rb.range_floor,
                      rb.count,
                      s.avg_rating,
                      s.stddev,
                      s.am_nz_ratio,
                      my.release_year
                  FROM rating_buckets rb
                  CROSS JOIN stats s
                  LEFT JOIN most_rated_year my ON TRUE
                  ORDER BY rb.range_floor;
                  """;
        
        var parameters = new List<NpgsqlParameter>()
        {
            new("@userId", userId)
        };

        var result = await connection.FetchListDynamicAsync(sql, parameters);

        var ratingCounts = new Dictionary<int, int>();

        decimal averageRating = 0;
        decimal standardDev = 0;
        decimal ratio = 0;
        short? mostRatedYear = null;

        foreach (var row in result)
        {
            var dict = (IDictionary<string, object>)row;

            ratingCounts[
                Convert.ToInt32(dict["range_floor"])
            ] = Convert.ToInt32(dict["count"]);

            averageRating = dict["avg_rating"] is DBNull
                ? 0
                : Convert.ToDecimal(dict["avg_rating"]);

            standardDev = dict["stddev"] is DBNull
                ? 0
                : Convert.ToDecimal(dict["stddev"]);

            ratio = dict["am_nz_ratio"] is DBNull
                ? 0
                : Convert.ToDecimal(dict["am_nz_ratio"]);

            mostRatedYear = dict["release_year"] is DBNull
                ? null
                : Convert.ToInt16(dict["release_year"]);
        }

        return new OutUserRatingStats
        {
            UserId = userId,
            RatingCounts = ratingCounts,
            AverageRating = averageRating,
            RatingStandardDev = standardDev,
            ArtistNameRatioAMvsNZ = ratio,
            MostRatedReleaseYear = mostRatedYear
        };
    }
}