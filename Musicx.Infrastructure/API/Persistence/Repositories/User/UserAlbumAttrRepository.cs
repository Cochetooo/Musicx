using System.Text;
using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.User;
using Musicx.Application.API.Persistence.Queries;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Contracts.Dto.Responses.Specifics.Ratings;
using Musicx.Contracts.Dto.Responses.User;
using Musicx.Infrastructure.API.Persistence.Builders;
using Musicx.Infrastructure.API.Persistence.Columns.Album;
using Musicx.Infrastructure.API.Persistence.Columns.Genre;
using Musicx.Infrastructure.API.Persistence.Columns.User;
using Musicx.Infrastructure.API.Persistence.Mappers;
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

    public async Task<long> CountGenreRatingsByUserIdAsync(long userId)
    {
        const string sql = """
                           SELECT COUNT(*) FROM (
                               SELECT g0.genre_id
                               FROM genres g0
                               JOIN album_genre ag0 ON g0.genre_id = ag0.album_genre_genre_id
                               JOIN albums al0 ON ag0.album_genre_album_id = al0.album_id
                               JOIN user_album_attrs uaa0 ON al0.album_id = uaa0.user_album_attrs_album_id
                               WHERE uaa0.user_album_attrs_user_id = @userId
                               GROUP BY g0.genre_id
                           ) grouped_genres
                           """;

        await using var conn = (NpgsqlConnection)connection.CreateConnection();
        await conn.OpenAsync();

        await using var command = new NpgsqlCommand(sql, conn);
        command.Parameters.Add(new NpgsqlParameter("@userId", userId));
        
        var count = await command.ExecuteScalarAsync();
        return count is null ? 0 : Convert.ToInt64(count);
    }

    /// <summary>
    /// Finds user album attributes with a typed query and returns the rows with the matching total.
    /// </summary>
    /// <param name="query">Typed query implementing <see cref="IFindQuery{T}"/>.</param>
    /// <param name="joinSpec">Optional join specification.</param>
    /// <param name="orderSpec">Optional order specification.</param>
    /// <param name="pagingOptions">Optional paging options.</param>
    /// <returns>A generic list containing the rows and the count for the same query.</returns>
    /// <since>0.7.4</since>
    public async Task<OutGenericList<OutUserAlbumAttribute>> FindAsync(
        IFindQuery<InUserAlbumAttribute>? query,
        IJoinSpecification<InUserAlbumAttribute>? joinSpec = null,
        OrderSpecification<InUserAlbumAttribute>? orderSpec = null,
        PagingOptions? pagingOptions = null)
    {
        if (query is not UserAlbumAttrFindQuery typedQuery)
        {
            return new OutGenericList<OutUserAlbumAttribute>();
        }
        
        var (sql, parameters) = builder.BuildFilteredQuery(
            typedQuery,
            joinSpec,
            orderSpec,
            pagingOptions,
            countOnly: false);
        
        var result = (await connection.FetchListDynamicAsync(sql, parameters))
            .Select(x => x.FromDicoToUserAlbumAttr())
            .ToList();
        
        var total = await CountAsync(typedQuery, joinSpec);

        return new OutGenericList<OutUserAlbumAttribute>
        {
            Items = result,
            Total = total
        };
    }
    
    public async Task<IReadOnlyList<OutUserGenreRating>> FindGenreRatingsByUserIdAsync(
        long userId,
        bool weighted = false,
        PagingOptions? pagingOptions = null)
    {
        var sql = weighted
            ? $"SELECT * FROM get_genre_ratings_by_user_id(@userId, @skip, @take);"
            : $"""
               SELECT
                   g0.{GenreColumns.Id} AS genre_id,
                   g0.{GenreColumns.CanonicalName} AS genre_name,
                   g0.{GenreColumns.Color} AS genre_color,
                   COUNT(DISTINCT al0.{AlbumColumns.Id}) AS album_count
               FROM genres g0
               JOIN album_genre ag0 ON g0.{GenreColumns.Id} = ag0.{AlbumGenreColumns.GenreId}
               JOIN albums al0 ON ag0.{AlbumGenreColumns.AlbumId} = al0.{AlbumColumns.Id}
               JOIN user_album_attrs uaa0 ON al0.{AlbumColumns.Id} = uaa0.{UserAlbumAttrColumns.AlbumId}
               WHERE uaa0.{UserAlbumAttrColumns.UserId} = @userId
               GROUP BY g0.{GenreColumns.Id}, g0.{GenreColumns.CanonicalName}, g0.{GenreColumns.Color}
               ORDER BY album_count DESC, g0.{GenreColumns.CanonicalName}
               OFFSET @skip LIMIT @take
               """;

        var parameters = new List<NpgsqlParameter>
        {
            new("@userId", userId),
            new("@skip", pagingOptions?.Skip ?? 0),
            new("@take", pagingOptions?.Take ?? 50)
        };

        var result = await connection.FetchListDynamicAsync(sql, parameters);

        return result
            .Select(row =>
            {
                var dict = (IDictionary<string, object>)row;

                return new OutUserGenreRating
                {
                    Id = Convert.ToInt64(dict["genre_id"]),
                    GenreName = Convert.ToString(dict["genre_name"]) ?? string.Empty,
                    GenreColor = dict["genre_color"] is DBNull ? null : Convert.ToString(dict["genre_color"]),
                    AlbumCount = Convert.ToInt64(dict["album_count"]),
                    WeightedScore = dict.TryGetValue("weighted_score", out var weightedScore) && weightedScore is not DBNull
                        ? Convert.ToDecimal(weightedScore)
                        : null,
                    WeightedPercent = dict.TryGetValue("weighted_percent", out var weightedPercent) && weightedPercent is not DBNull
                        ? Convert.ToDecimal(weightedPercent)
                        : null
                };
            })
            .ToList();
    }
    
    public async Task<OutGenericList<OutUserAlbumAttribute>> FindReviewsByAlbumIdAsync(
        long albumId,
        PagingOptions? pagingOptions = null)
    {
        var sql = $"""
                   SELECT uaa0.*, us0.*, al0.*, ars0.*
                   FROM user_album_attrs uaa0
                   INNER JOIN users us0
                       ON us0.{UserColumns.Id} = uaa0.{UserAlbumAttrColumns.UserId}
                   INNER JOIN albums al0
                       ON al0.{AlbumColumns.Id} = uaa0.{UserAlbumAttrColumns.AlbumId}
                   LEFT JOIN review_sources ars0
                       ON ars0.review_source_id = uaa0.{UserAlbumAttrColumns.ReviewSourceId}
                   WHERE uaa0.{UserAlbumAttrColumns.AlbumId} = @albumId
                       AND uaa0.{UserAlbumAttrColumns.Review} IS NOT NULL
                       AND btrim(uaa0.{UserAlbumAttrColumns.Review}) <> ''
                   ORDER BY COALESCE(uaa0.{UserAlbumAttrColumns.ReviewPostedAt}, uaa0.{UserAlbumAttrColumns.UpdatedAt}) DESC
                   OFFSET @skip LIMIT @take
                   """;

        var countSql = $"""
                        SELECT COUNT(*)
                        FROM user_album_attrs uaa0
                        WHERE uaa0.{UserAlbumAttrColumns.AlbumId} = @albumId
                            AND uaa0.{UserAlbumAttrColumns.Review} IS NOT NULL
                            AND btrim(uaa0.{UserAlbumAttrColumns.Review}) <> ''
                        """;

        var parameters = new List<NpgsqlParameter>
        {
            new("@albumId", albumId),
            new("@skip", pagingOptions?.Skip ?? 0),
            new("@take", pagingOptions?.Take ?? 5)
        };

        var items = (await connection.FetchListDynamicAsync(sql, parameters))
            .Select(x => x.FromDicoToUserAlbumAttr())
            .ToList();

        await using var conn = (NpgsqlConnection)connection.CreateConnection();
        await conn.OpenAsync();
        await using var command = new NpgsqlCommand(countSql, conn);
        command.Parameters.Add(new NpgsqlParameter("@albumId", albumId));

        var count = await command.ExecuteScalarAsync();

        return new OutGenericList<OutUserAlbumAttribute>
        {
            Items = items,
            Total = count is null ? 0 : Convert.ToInt64(count)
        };
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
    
    /// <summary>
    /// Counts user album attributes matching a typed query.
    /// </summary>
    /// <param name="findQuery">Typed query used to constrain the count.</param>
    /// <param name="spec">Optional join specification.</param>
    /// <returns>The number of rows matching the query.</returns>
    /// <since>0.7.4</since>
    public async Task<long> CountAsync(
        IFindQuery<InUserAlbumAttribute>? findQuery = null,
        IJoinSpecification<InUserAlbumAttribute>? spec = null)
    {
        var query = findQuery as UserAlbumAttrFindQuery ?? new UserAlbumAttrFindQuery();

        var (sql, parameters) = builder.BuildFilteredQuery(
            query: query,
            joinSpec: spec,
            orderSpec: null,
            pagingOptions: null,
            countOnly: true);

        await using var conn = (NpgsqlConnection)connection.CreateConnection();
        await conn.OpenAsync();
        await using var command = new NpgsqlCommand(sql, conn);

        command.Parameters.AddRange(parameters.ToArray());

        _logger.LogDebug(SqlHelper.InterpolateQuery(sql, parameters));

        try
        {
            var result = await command.ExecuteScalarAsync();
            return result is null ? 0 : Convert.ToInt64(result);
        }
        catch (Exception)
        {
            _logger.LogError("❌ Could not execute count command for table user_album_attrs.");
            return -1;
        }
    }
    
    public async Task<long> SaveAsync(InUserAlbumAttribute entity)
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
            throw new RepositoryException("❌ SAVE Album : Could not persist.", ex, _logger);
        }
        
        return -1;
    }
    
    public async Task<IReadOnlyList<OutUserYearlyRating>> GetUserYearlyRatingsAsync(
        int bucketSize = 5,
        long? genreId = null,
        long? userId = null)
    {
        if (bucketSize <= 0)
        {
            bucketSize = 5;
        }

        var sql = """
                  SELECT *
                  FROM get_user_album_yearly_ratings(@bucketSize, @genreId, @userId);
                  """;

        var parameters = new List<NpgsqlParameter>
        {
            new("@userId", userId ?? (object)DBNull.Value),
            new("@bucketSize", bucketSize),
            new("@genreId", genreId ?? (object)DBNull.Value)
        };

        var result = await connection.FetchListDynamicAsync(sql, parameters);

        return result
            .Select(row =>
            {
                var dict = (IDictionary<string, object>)row;

                return new OutUserYearlyRating
                {
                    YearBucketStart = Convert.ToInt32(dict["year_bucket_start"]),
                    YearBucketEnd = Convert.ToInt32(dict["year_bucket_end"]),
                    YearBucketLabel = Convert.ToString(dict["year_bucket_label"]) ?? string.Empty,
                    GenreId = dict["genre_id"] is DBNull ? null : Convert.ToInt64(dict["genre_id"]),
                    GenreName = dict["genre_name"] is DBNull ? null : Convert.ToString(dict["genre_name"]),
                    GenreColor = dict["genre_color"] is DBNull ? null : Convert.ToString(dict["genre_color"]),
                    AverageRating = dict["average_rating"] is DBNull ? 0 : Convert.ToDecimal(dict["average_rating"]),
                    RatingCount = Convert.ToInt32(dict["rating_count"])
                };
            })
            .ToList();
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