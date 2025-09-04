using System.Linq.Expressions;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Specifications;
using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Infrastructure.API.Persistence.Builders;
using Musicx.Infrastructure.API.Persistence.Columns;
using Musicx.Infrastructure.API.Persistence.Mappers;
using Musicx.Infrastructure.Shared.Exceptions;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;
using ISongRepository = Musicx.Application.Api.Interfaces.Persistence.ISongRepository;

namespace Musicx.Infrastructure.API.Persistence.Repositories;

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

    public async Task<OutSong?> FindByIdAsync(long id, IQuerySpecification<InSong>? songQuerySpecification = null)
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

    public async Task<List<OutSong>> FindAsync(int skip = 0, int take = 100,
        IQuerySpecification<InSong>? songQuerySpecification = null, string? filter = null)
    {
        var sql = builder.BuildSelect(songQuerySpecification);
        var parameters = new List<NpgsqlParameter>();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            sql += $" WHERE similarity(s0.{SongColumns.Title}, @filter) > 0.4";
            parameters.Add(new NpgsqlParameter("@filter", filter));
        }
        
        sql += builder.BuildGroupBy(songQuerySpecification);
        
        if (!string.IsNullOrWhiteSpace(filter))
        {
            sql += builder.BuildOrderBy($"similarity(s0.{SongColumns.Title}, @filter) DESC");
        }
        else
        {
            sql += builder.BuildOrderBy($"s0.{SongColumns.Title}");
        }
        
        sql += " OFFSET @skip LIMIT @take";
        
        parameters.Add(new NpgsqlParameter("@skip", skip));
        parameters.Add(new NpgsqlParameter("@take", take));

        var result = await connection.FetchListDynamicAsync(sql, parameters);

        return result
            .Select(x => x.FromDicoToSong())
            .ToList();
    }

    public async Task<List<OutSong>> FindIn(IEnumerable<long> ids, IQuerySpecification<InSong>? songQuerySpecification = null)
    {
        var idList = ids.ToArray();
        if (0 == idList.Length)
        {
            return [];
        }
        
        var stringIds = string.Join(",", idList);
        var sql = builder.BuildSelect(songQuerySpecification);
        sql += $" WHERE s0.{SongColumns.Id} IN ({stringIds})" +
               builder.BuildGroupBy(songQuerySpecification) +
               $" ORDER BY s0.{SongColumns.Title}";
        
        var result = await connection.FetchListDynamicAsync(sql, []);
        
        return result
            .Select(x => x.FromDicoToSong())
            .ToList();
    }

    public async Task<long> GetCountAsync()
        => await connection.Count("songs");

    public async Task<long> SaveAsync(InSong entity)
    {
        await using var conn = connection.CreateConnection();
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
        
        await using var conn = connection.CreateConnection();
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