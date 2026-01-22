using System.Text;
using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.Song;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Song;
using Musicx.Contracts.Dto.Responses;
using Musicx.Infrastructure.API.Persistence.Builders;
using Musicx.Infrastructure.API.Persistence.Columns.Song;
using Musicx.Infrastructure.API.Persistence.Connection;
using Musicx.Infrastructure.API.Persistence.Mappers;
using Musicx.Infrastructure.API.Persistence.Specifications.Song;
using Musicx.Infrastructure.Shared.Exceptions;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Repositories.Song;

internal sealed class SongRepository(
    IDbConnectionProvider connection,
    SqlBuilder<InSong> builder,
    ILoggerProvider loggerProvider) : ISongRepository
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(SongRepository));
    
    public async Task DeleteAsync(long id)
    {
        const string sql = $"DELETE FROM songs WHERE {SongColumns.Id} = @id";

        var parameters = new List<NpgsqlParameter>
        {
            new("@id", id)
        };

        await connection.ExecuteTransactionAsync((sql, parameters));
    }

    public async Task DeleteAllAsync(IEnumerable<long> ids)
    {
        var stringIds = string.Join(",", ids);
        const string sql = $"DELETE FROM songs WHERE {SongColumns.Id} IN (@ids)";
        
        var parameters = new List<NpgsqlParameter>
        {
            new("@Ids", stringIds)
        };
        
        await connection.ExecuteTransactionAsync((sql, parameters));
    }

    public async Task<OutSong?> FindByIdAsync(long id, IJoinSpecification<InSong>? songQuerySpecification = null)
    {
        var sql = new StringBuilder(builder.BuildSelect(songQuerySpecification));
        sql.Append($" WHERE s0.{SongColumns.Id} = @id")
            .Append(builder.BuildGroupBy(songQuerySpecification));
        
        var parameters = new List<NpgsqlParameter>
        {
            new("@id", id)
        };

        var result = await connection.FetchListDynamicAsync(sql.ToString(), parameters);

        return result
            .SingleOrDefault()?
            .FromDicoToSong();
    }

    public async Task<List<OutSong>> FindAsync(
        bool? filterExact = false, double? filterSimilitude = 0.4, string? filter = null,
        IJoinSpecification<InSong>? joinSpec = null,
        OrderSpecification<InSong>? orderSpec = null,
        PagingOptions? pagingOptions = null)
    {
        var sql = builder.BuildSelect(joinSpec);
        var parameters = new List<NpgsqlParameter>();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            builder.Filter(
                sql: ref sql, 
                column: $"s0.{SongColumns.Title}", 
                filter: filter,
                parameters: parameters, 
                filterExact: filterExact, 
                filterSimilitude: filterSimilitude
            );
        }
        
        sql += builder.BuildGroupBy(joinSpec);
        
        if (orderSpec is not null)
        {
            sql += builder.BuildOrderBy(orderSpec);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(filter))
            {
                sql += $" ORDER BY similarity(s0.{SongColumns.Title}, @filter) DESC";
            }
            else
            {
                sql += $" ORDER BY s0.{SongColumns.Title}";
            }
        }
        
        sql +=  " OFFSET @skip LIMIT @take";
        
        parameters.Add(new NpgsqlParameter("@skip", pagingOptions?.Skip ?? 0));
        parameters.Add(new NpgsqlParameter("@take", pagingOptions?.Take ?? 200));

        var result = await connection.FetchListDynamicAsync(sql, parameters);

        return result
            .Select(x => x.FromDicoToSong())
            .ToList();
    }

    public async Task<List<OutSong>> FindByAlbumIdAsync(long albumId, 
        IJoinSpecification<InSong>? joinSpec = null,
        OrderSpecification<InSong>? orderSpec = null)
    {
        var sql = builder.BuildSelect(joinSpec);
        sql += $" WHERE s0.{SongColumns.AlbumId} = @albumId" +
               builder.BuildGroupBy(joinSpec);

        if (orderSpec is not null)
        {
            sql += builder.BuildOrderBy(orderSpec);
        }
        else
        {
            sql += $" ORDER BY s0.{SongColumns.TrackNumber}, s0.{SongColumns.Title}";
        }

        var parameters = new List<NpgsqlParameter>
        {
            new("@albumId", albumId)
        };
        
        var result = await connection.FetchListDynamicAsync(sql, parameters);

        return result
            .Select(x => x.FromDicoToSong())
            .ToList();
    }

    public async Task<List<OutSong>> FindIn(IEnumerable<long> ids, 
        IJoinSpecification<InSong>? joinSpec = null,
        OrderSpecification<InSong>? orderSpec = null)
    {
        var idList = ids.ToArray();
        if (0 == idList.Length)
        {
            return [];
        }
        
        var stringIds = string.Join(",", idList);
        var sql = builder.BuildSelect(joinSpec);
        sql += $" WHERE s0.{SongColumns.Id} IN ({stringIds})" +
               builder.BuildGroupBy(joinSpec);

        if (orderSpec is not null)
        {
            sql += builder.BuildOrderBy(orderSpec);
        }
        
        var result = await connection.FetchListDynamicAsync(sql, []);
        
        return result
            .Select(x => x.FromDicoToSong())
            .ToList();
    }

    public async Task<long> GetCountAsync()
        => await connection.Count("songs");

    public async Task<long> SaveAsync(InSong entity)
    {
        await using var conn = (NpgsqlConnection)connection.CreateConnection();
        await conn.OpenAsync();
        
        await using var transaction = await conn.BeginTransactionAsync();
        
        try
        {
            if (0 == entity.Id)
            {
                var result = await builder.ExecuteInsert(entity, conn, transaction);
                if (result is long l)
                {
                    entity.Id = l;
                }
                else
                {
                    _logger.LogWarning("⚠️ Result from Insert is not long: {result}", result?.ToString());
                }
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
            throw new RepositoryException("❌ SAVE Song : Could not persist.", ex, _logger);
        }
        
        return entity.Id;
    }

    public async Task<List<long>> SaveAllAsync(IEnumerable<InSong> entities)
    {
        var idList = new List<long>();
        
        await using var conn = (NpgsqlConnection)connection.CreateConnection();
        await conn.OpenAsync();
        
        await using var transaction = await conn.BeginTransactionAsync();
        
        try
        {
            foreach (var entity in entities)
            {
                if (0 == entity.Id)
                {
                    var result = await builder.ExecuteInsert(entity, conn, transaction);
                    if (result is long l)
                    {
                        entity.Id = l;
                    }
                    else
                    {
                        _logger.LogWarning("⚠️ Result from Insert is not long: {result}", result?.ToString());
                    }
                }
                else
                {
                    await builder.ExecuteUpdate(entity, conn, transaction);
                }
                
                idList.Add(entity.Id);
            }
            
            await transaction.CommitAsync();
        } 
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new RepositoryException("❌ SAVE ALL Song : Could not persist.", ex, _logger);
        }
        
        return idList;
    }
}