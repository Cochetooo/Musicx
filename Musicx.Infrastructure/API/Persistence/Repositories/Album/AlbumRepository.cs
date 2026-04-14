using System.Text;
using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.Album;
using Musicx.Application.API.Persistence.Queries;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Album;
using Musicx.Contracts.Dto.Requests.Specifics;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Infrastructure.API.Persistence.Builders;
using Musicx.Infrastructure.API.Persistence.Builders.Album;
using Musicx.Infrastructure.API.Persistence.Columns.Album;
using Musicx.Infrastructure.API.Persistence.Columns.Artist;
using Musicx.Infrastructure.API.Persistence.Connection;
using Musicx.Infrastructure.API.Persistence.Mappers;
using Musicx.Infrastructure.API.Persistence.Specifications.Album;
using Musicx.Infrastructure.Shared.Exceptions;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Repositories.Album;

internal sealed class AlbumRepository(
    IDbConnectionProvider connection,
    SqlBuilder<InAlbum> builder,
    ILoggerProvider loggerProvider) : IAlbumRepository
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(AlbumRepository));
    
    public async Task DeleteAsync(long id)
    {
        const string sql = $"DELETE FROM albums WHERE {AlbumColumns.Id} = @id";
        
        var parameters = new List<NpgsqlParameter>
        {
            new("@id", id)
        };

        await connection.ExecuteTransactionAsync((sql, parameters));
    }

    public async Task DeleteAllAsync(IEnumerable<long> ids)
    {
        var stringIds = string.Join(",", ids);
        const string sql = $"DELETE FROM albums WHERE {AlbumColumns.Id} IN (@ids)";
        
        var parameters = new List<NpgsqlParameter>
        {
            new("@Ids", stringIds)
        };
        
        await connection.ExecuteTransactionAsync((sql, parameters));
    }

    public async Task<OutAlbum?> FindOneByIdAsync(
        long id, 
        IJoinSpecification<InAlbum>? joinSpec = null)
    {
        var sql = new StringBuilder(builder.BuildSelect(joinSpec));
        sql.Append($" WHERE al0.{AlbumColumns.Id} = @id")
            .Append(builder.BuildGroupBy(joinSpec));
        
        var parameters = new List<NpgsqlParameter>
        {
            new("@id", id)
        };

        var result = await connection.FetchListDynamicAsync(sql.ToString(), parameters);

        try
        {
            return result
                .SingleOrDefault()?
                .FromDicoToAlbum();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            _logger.LogCritical(e.StackTrace);
            throw new ApplicationException();
        }
    }
    
    public async Task<List<OutAlbum>> FindByArtistIdAsync(long artistId, 
        IJoinSpecification<InAlbum>? joinSpec = null,
        OrderSpecification<InAlbum>? orderSpec = null)
    {
        var sql = builder.BuildSelect(joinSpec);
        
        sql += $" WHERE al0.{AlbumColumns.ArtistId} = @artistId" + 
               builder.BuildGroupBy(joinSpec);

        if (orderSpec is not null)
        {
            sql += builder.BuildOrderBy(orderSpec);
        }
                
        var parameters = new List<NpgsqlParameter>
        {
            new("@artistId", artistId)
        };
        
        var result = await connection.FetchListDynamicAsync(sql, parameters);
        
        return result
            .Select(x => x.FromDicoToAlbum())
            .ToList();
    }
    
    public async Task<Dictionary<long, List<OutAlbum>>> FindByArtistIdsAsync(
        IEnumerable<long> artistIds,
        IJoinSpecification<InAlbum>? joinSpec = null,
        OrderSpecification<InAlbum>? orderSpecification = null)
    {
        var ids = artistIds.Distinct().ToArray();
        if (ids.Length == 0)
        {
            return new Dictionary<long, List<OutAlbum>>();
        }

        var sql = builder.BuildSelect(joinSpec);
        sql += $" WHERE al0.{AlbumColumns.ArtistId} = ANY(@artistIds)" + builder.BuildGroupBy(joinSpec);

        if (orderSpecification is not null)
        {
            sql += builder.BuildOrderBy(orderSpecification);
        }

        var parameters = new List<NpgsqlParameter>
        {
            new("@artistIds", ids)
        };

        var result = await connection.FetchListDynamicAsync(sql, parameters);

        return result
            .Select(x => x.FromDicoToAlbum())
            .Where(x => x.ArtistId is not null)
            .GroupBy(x => x.ArtistId!.Value)
            .ToDictionary(x => x.Key, x => x.ToList());
    }
    
    public async Task<List<OutAlbum>> FindByGenreIdAsync(long genreId, 
        int genreOptions,
        IJoinSpecification<InAlbum>? joinSpec = null,
        OrderSpecification<InAlbum>? orderSpec = null,
        PagingOptions? pagingOptions = null)
    {
        const int validMask = GenreOptions.PrimaryGenre | GenreOptions.InfluenceGenre;

        if ((genreOptions & ~validMask) != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(genreOptions),
                $"Invalid genreOptions: {genreOptions}. Must be a combination of GenreOptions.PrimaryGenre (0x1) and/or InfluenceGenre (0x2).");
        }
        
        /*
         * Assure that primary & influence genres are retrieved because this endpoint must need it.
         */

        if (joinSpec is AlbumJoinSpecification albumSpec)
        {
            albumSpec.IncludePrimaryGenres = true;
            albumSpec.IncludeInfluenceGenres = true;
        }
        else
        {
            joinSpec = new AlbumJoinSpecification
            {
                IncludePrimaryGenres = true,
                IncludeInfluenceGenres = true
            };
        }
        
        var sql = builder.BuildSelect(joinSpec);
        var whereConditions = new List<string>();

        if ((genreOptions & GenreOptions.PrimaryGenre) != 0)
        {
            whereConditions.Add($"apg.{AlbumGenreColumns.GenreId} = @genreId");
        }
        
        if ((genreOptions & GenreOptions.InfluenceGenre) != 0)
        {
            whereConditions.Add($"aig.{AlbumInfluenceColumns.GenreId} = @genreId");
        }
        
        sql += $"""
                   WHERE {string.Join(" OR ", whereConditions)}
                  {builder.BuildGroupBy(joinSpec)}
                  """;

        if (orderSpec is not null)
        {
            sql += builder.BuildOrderBy(orderSpec);
        }

        sql += " OFFSET @skip LIMIT @take";
        
        var parameters = new List<NpgsqlParameter>
        {
            new("@genreId", genreId),
            new("@skip", pagingOptions?.Skip ?? 0),
            new("@take", pagingOptions?.Take ?? 200),
        };
        
        var result = await connection.FetchListDynamicAsync(sql, parameters);
        
        return result
            .Select(x => x.FromDicoToAlbum())
            .ToList();
    }

    public async Task<List<OutAlbum>> FindByChart(AlbumChartQuery query)
    {
        var sql = builder.BuildSelect(new AlbumJoinSpecification
        {
            IncludeArtist = true,
            IncludeStats = true,
        });

        var (whereClause, orderClause, parameters) = ((AlbumSqlBuilder)builder).BuildChartQuery(query);

        sql += $"""
                {whereClause}
                {orderClause}
                LIMIT @take OFFSET @skip;
                """;
        
        parameters.Add(new NpgsqlParameter("@take", query.Take));
        parameters.Add(new NpgsqlParameter("@skip", query.Skip));
        
        // @TODO count total
        
        var result = await connection.FetchListDynamicAsync(sql, parameters);
        
        return result
            .Select(x => x.FromDicoToAlbum())
            .ToList();
    }
    
    public async Task<OutGenericList<OutAlbum>> FindSimilarAsync(
        long albumId,
        OrderSpecification<InAlbum>? orderSpec = null,
        PagingOptions? pagingOptions = null)
    {
        var sql = $"""
                   SELECT al0.*, ar0.*, alst0.*, sc.similarity_score
                   FROM get_similar_albums(@albumId) sc
                   INNER JOIN albums al0 ON al0.{AlbumColumns.Id} = sc.album_id
                   LEFT JOIN artists ar0 ON ar0.{ArtistColumns.Id} = al0.{AlbumColumns.ArtistId}
                   LEFT JOIN album_rating_stats alst0 ON alst0.{AlbumRatingStatColumns.AlbumId} = al0.{AlbumColumns.Id}
                   """;

        if (orderSpec is not null)
        {
            sql += builder.BuildOrderBy(orderSpec);
        }
        else
        {
            sql += $" ORDER BY sc.similarity_score DESC, al0.{AlbumColumns.Name}";
        }

        sql += " OFFSET @skip LIMIT @take;";

        var parameters = new List<NpgsqlParameter>
        {
            new("@albumId", albumId),
            new("@skip", pagingOptions?.Skip ?? 0),
            new("@take", pagingOptions?.Take ?? 6)
        };

        var items = (await connection.FetchListDynamicAsync(sql, parameters))
            .Select(x => x.FromDicoToAlbum())
            .ToList();

        const string countSql = "SELECT COUNT(*) FROM get_similar_albums(@albumId);";
        await using var conn = (NpgsqlConnection)connection.CreateConnection();
        await conn.OpenAsync();
        await using var command = new NpgsqlCommand(countSql, conn);
        command.Parameters.Add(new NpgsqlParameter("@albumId", albumId));
        var count = await command.ExecuteScalarAsync();

        return new OutGenericList<OutAlbum>
        {
            Items = items,
            Total = count is null ? 0 : Convert.ToInt64(count)
        };
    }

    public async Task<OutGenericList<OutAlbum>> FindAsync(
        IFindQuery<InAlbum>? query,
        IJoinSpecification<InAlbum>? joinSpec = null,
        OrderSpecification<InAlbum>? orderSpec = null,
        PagingOptions? pagingOptions = null)
    {
        if (query is not AlbumFindQuery typedQuery)
        {
            return new OutGenericList<OutAlbum>();
        }

        var (sql, parameters) = builder.BuildFilteredQuery(
            typedQuery,
            joinSpec,
            orderSpec,
            pagingOptions,
            countOnly: false
        );

        var result = (await connection.FetchListDynamicAsync(sql, parameters))
            .Select(x => x.FromDicoToAlbum())
            .ToList();

        var total = await CountAsync(typedQuery, joinSpec);

        return new OutGenericList<OutAlbum>
        {
            Items = result,
            Total = total
        };
    }

    public async Task<List<OutAlbum>> FindInAsync(IEnumerable<long> ids,
        IJoinSpecification<InAlbum>? joinSpec = null,
        OrderSpecification<InAlbum>? orderSpec = null)
    {
        var idList = ids.ToArray();
        if (0 == idList.Length)
        {
            return [];
        }
        
        var stringIds = string.Join(",", idList);
        var sql = builder.BuildSelect(joinSpec);
        sql += $" WHERE al0.{AlbumColumns.Id} IN ({stringIds})" +
               builder.BuildGroupBy(joinSpec);

        if (orderSpec is not null)
        {
            sql += builder.BuildOrderBy(orderSpec);
        }
        
        var result = await connection.FetchListDynamicAsync(sql, []);
        
        return result
            .Select(x => x.FromDicoToAlbum())
            .ToList();
    }

    public async Task<long> CountAsync(IFindQuery<InAlbum>? query = null,
        IJoinSpecification<InAlbum>? spec = null)
    {
        var typedQuery = query as AlbumFindQuery ?? new AlbumFindQuery();
        
        var (sql, parameters) = builder.BuildFilteredQuery(
            query: typedQuery,
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
            _logger.LogError("❌ Could not execute count command for table albums.");
            return -1;
        }
    }

    public async Task<long> GetCountByGenreIdAsync(long genreId)
    {
        await using var conn = (NpgsqlConnection)connection.CreateConnection();
        await conn.OpenAsync();

        var parameters = new List<NpgsqlParameter>()
        {
            new("@genreId", genreId),
        };

        var sql = $"SELECT COUNT(*) FROM albums al0 "
                  + $"JOIN album_genre ag0 ON al0.{AlbumColumns.Id} = ag0.{AlbumGenreColumns.AlbumId} "
                  + $" WHERE ag0.{AlbumGenreColumns.GenreId} = @genreId";

        await using var command = new NpgsqlCommand(sql, conn);
        
        command.Parameters.AddRange(parameters.ToArray());
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(sql, parameters));

        try
        {
            return Convert.ToInt32(await command.ExecuteScalarAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError("❌ Could not execute count by genre command for table album: " + ex.Message
                + "\n" + ex.StackTrace);
            return -1;
        }
    }

    public async Task<long> SaveAsync(InAlbum entity)
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
        
        return entity.Id;
    }

    public async Task<List<long>> SaveAllAsync(IEnumerable<InAlbum> entities)
    {
        var idList = new List<long>();
        
        await using var conn = (NpgsqlConnection)connection.CreateConnection();
        await conn.OpenAsync();
        
        await using var transaction = await conn.BeginTransactionAsync();
        
        try
        {
            foreach (var entity in entities)
            {
                await builder.ExecuteUpsert(entity, conn, transaction);
                idList.Add(entity.Id);
            }
            
            await transaction.CommitAsync();
        } 
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new RepositoryException("❌ SAVE ALL Album : Could not persist.", ex, _logger);
        }
        
        return idList;
    }
}