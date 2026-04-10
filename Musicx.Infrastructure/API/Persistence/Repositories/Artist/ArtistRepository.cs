using System.Text;
using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.Artist;
using Musicx.Application.API.Persistence.Queries;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Artist;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Artist;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Contracts.Enums;
using Musicx.Infrastructure.API.Persistence.Builders;
using Musicx.Infrastructure.API.Persistence.Columns.Album;
using Musicx.Infrastructure.API.Persistence.Columns.Artist;
using Musicx.Infrastructure.API.Persistence.Columns.Genre;
using Musicx.Infrastructure.API.Persistence.Connection;
using Musicx.Infrastructure.API.Persistence.Mappers;
using Musicx.Infrastructure.Shared.Exceptions;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Repositories.Artist;

internal sealed class ArtistRepository(
    IDbConnectionProvider connection,
    SqlBuilder<InArtist> builder,
    ILoggerProvider loggerProvider) : IArtistRepository
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(ArtistRepository));
    
    public async Task DeleteAsync(long id)
    {
        const string sql = $"DELETE FROM artists WHERE {ArtistColumns.Id} = @id";
        var parameters = new List<NpgsqlParameter>
        {
            new("@id", id)
        };

        await connection.ExecuteTransactionAsync((sql, parameters));
    }

    public async Task DeleteAllAsync(IEnumerable<long> ids)
    {
        var stringIds = string.Join(",", ids);
        const string sql = $"DELETE FROM artists WHERE {ArtistColumns.Id} IN (@ids)";
        
        var parameters = new List<NpgsqlParameter>
        {
            new("@ids", stringIds)
        };
        
        await connection.ExecuteTransactionAsync((sql, parameters));
    }

    public async Task<OutArtist?> FindOneByIdAsync(long id, IJoinSpecification<InArtist>? artistQuerySpecification = null)
    {
        var sql = new StringBuilder(builder.BuildSelect(artistQuerySpecification));
        sql.Append($" WHERE ar0.{ArtistColumns.Id} = @id");
        
        var parameters = new List<NpgsqlParameter>
        {
            new("@id", id)
        };
        
        var result = await connection.FetchListDynamicAsync(sql.ToString(), parameters);

        return result
            .SingleOrDefault()?
            .FromDicoToArtist();
    }
    
    public async Task<IReadOnlyList<OutArtist>> FindByGenreIdAsync(long genreId, PagingOptions? pagingOptions = null)
    {
        var sql = $"""
                   SELECT ar0.*, COUNT(DISTINCT al0.{AlbumColumns.Id}) AS album_count 
                   FROM artists ar0 
                   JOIN albums al0 ON ar0.{ArtistColumns.Id} = al0.{AlbumColumns.ArtistId} 
                   JOIN album_genre ag0 ON al0.{AlbumColumns.Id} = ag0.{AlbumGenreColumns.AlbumId} 
                   JOIN genres g0 ON ag0.{AlbumGenreColumns.GenreId} = g0.{GenreColumns.Id} 
                   WHERE g0.{GenreColumns.Id} = @genreId AND al0.{AlbumColumns.ReleaseType} = {(int)ReleaseType.Lp} 
                   GROUP BY ar0.artist_id 
                   ORDER BY album_count DESC 
                   OFFSET @skip LIMIT @take
                   """;

        var parameters = new List<NpgsqlParameter>
        {
            new("@genreId", genreId),
            new("@skip", pagingOptions?.Skip ?? 0),
            new("@take", pagingOptions?.Take ?? 200)
        };
        
        var result = await connection.FetchListDynamicAsync(sql, parameters);

        return result
            .Select(x => x.FromDicoToArtist())
            .ToList();
    }

    public async Task<OutGenericList<OutArtist>> FindAsync(
        IFindQuery<InArtist>? query,
        IJoinSpecification<InArtist>? joinSpec = null,
        OrderSpecification<InArtist>? orderSpec = null,
        PagingOptions? pagingOptions = null)
    {
        try
        {
            if (query is not ArtistFindQuery typedQuery)
            {
                return new OutGenericList<OutArtist>();
            }

            var (sql, parameters) = builder.BuildFilteredQuery(typedQuery, joinSpec, orderSpec, pagingOptions, false);
            var result = await connection.FetchListDynamicAsync(sql, parameters);
            var total = await CountAsync(typedQuery, joinSpec);

            return new OutGenericList<OutArtist>
            {
                Items = result.Select(x => x.FromDicoToArtist()).ToList(),
                Total = total
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new OutGenericList<OutArtist>
            {
                Items = [],
                Total = 0
            };
        }
    }

    public async Task<List<OutArtist>> FindInAsync(IEnumerable<long> ids,
        IJoinSpecification<InArtist>? joinSpec = null,
        OrderSpecification<InArtist>? orderSpec = null)
    {
        var idList = ids.ToArray();
        if (0 == idList.Length)
        {
            return [];
        }

        var stringIds = string.Join(",", idList);
        var sql = builder.BuildSelect(joinSpec);
        sql += $" WHERE ar0.{ArtistColumns.Id} IN ({stringIds})";

        if (orderSpec is not null)
        {
            sql += builder.BuildOrderBy(orderSpec);
        }

        var result = await connection.FetchListDynamicAsync(sql, []);
        
        return result
            .Select(x => x.FromDicoToArtist())
            .ToList();
    }

    public async Task<long> CountAsync(IFindQuery<InArtist>? query = null,
        IJoinSpecification<InArtist>? spec = null)
    {
        var typedQuery = query as ArtistFindQuery ?? new ArtistFindQuery();
        var (sql, parameters) = builder.BuildFilteredQuery(typedQuery, spec, null, null, true);
        
        await using var conn = (NpgsqlConnection)connection.CreateConnection();
        await conn.OpenAsync();
        await using var command = new NpgsqlCommand(sql, conn);
        
        command.Parameters.AddRange(parameters.ToArray());
        
        try
        {
            var result = await command.ExecuteScalarAsync();
            return result is null ? 0 : Convert.ToInt64(result);
        }
        catch (Exception)
        {
            _logger.LogError("❌ Could not execute count command for table artists.");
            return -1;
        }
    }

    public async Task<long> GetCountByGenreIdAsync(long genreId)
    {
        await using var conn = (NpgsqlConnection)connection.CreateConnection();
        await conn.OpenAsync();

        var parameters = new List<NpgsqlParameter>
        {
            new("@genreId", genreId),
        };

        var sql = $"""
                   SELECT COUNT(DISTINCT ar0.artist_id) 
                   FROM artists ar0 
                   JOIN albums al0 ON ar0.{ArtistColumns.Id} = al0.{AlbumColumns.ArtistId} 
                   JOIN album_genre ag0 ON al0.{AlbumColumns.Id} = ag0.{AlbumGenreColumns.AlbumId} 
                   JOIN genres g0 ON ag0.{AlbumGenreColumns.GenreId} = g0.{GenreColumns.Id} 
                   WHERE g0.{GenreColumns.Id} = @genreId AND al0.{AlbumColumns.ReleaseType} = {(int)ReleaseType.Lp} 
                   """;

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

    public async Task<long> SaveAsync(InArtist entity)
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
            throw new RepositoryException($"❌ SAVE Artist : Could not persist: {ex.Message}", ex, _logger);
        }

        return entity.Id;
    }

    public async Task<List<long>> SaveAllAsync(IEnumerable<InArtist> entities)
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
            throw new RepositoryException("❌ SAVE ALL Artist : Could not persist.", ex, _logger);
        }
        
        return idList;
    }
}