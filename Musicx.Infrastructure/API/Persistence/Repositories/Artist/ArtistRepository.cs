using System.Text;
using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.Artist;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Artist;
using Musicx.Contracts.Dto.Responses;
using Musicx.Infrastructure.API.Persistence.Builders;
using Musicx.Infrastructure.API.Persistence.Columns.Artist;
using Musicx.Infrastructure.API.Persistence.Connection;
using Musicx.Infrastructure.API.Persistence.Mappers;
using Musicx.Infrastructure.Shared.Exceptions;
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

    public async Task<OutArtist?> FindByIdAsync(long id, IJoinSpecification<InArtist>? artistQuerySpecification = null)
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

    public async Task<List<OutArtist>> FindAsync(
        bool? filterExact = null, double? filterSimilitude = 0.4, string? filter = null, 
        IJoinSpecification<InArtist>? joinSpec = null,
        OrderSpecification<InArtist>? orderSpec = null,
        PagingOptions? pagingOptions = null)
    {
        var sql = builder.BuildSelect(joinSpec);
        
        var parameters = new List<NpgsqlParameter>();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            builder.Filter(
                sql: ref sql, 
                columns: [$"ar0.{ArtistColumns.Name}", $"ar0.{ArtistColumns.Alias}"], 
                filter: filter,
                parameters: parameters, 
                filterExact: filterExact, 
                filterSimilitude: filterSimilitude
            );
        }

        if (orderSpec is not null)
        {
            sql += builder.BuildOrderBy(orderSpec);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(filter))
            {
                sql += $" ORDER BY similarity(ar0.{ArtistColumns.Name}, @filter) DESC";
            }
            else
            {
                sql += $" ORDER BY ar0.{ArtistColumns.Name}";
            }
        }
        
        sql +=  " OFFSET @skip LIMIT @take";
        
        parameters.Add(new NpgsqlParameter("@skip", pagingOptions?.Skip ?? 0));
        parameters.Add(new NpgsqlParameter("@take", pagingOptions?.Take ?? 200));

        var result = await connection.FetchListDynamicAsync(sql, parameters);

        return result
            .Select(x => x.FromDicoToArtist())
            .ToList();
    }

    public async Task<List<OutArtist>> FindIn(IEnumerable<long> ids, 
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

    public async Task<long> GetCountAsync()
        => await connection.Count("artists");

    public async Task<long> SaveAsync(InArtist entity)
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
            throw new RepositoryException($"❌ SAVE Artist : Could not persist: {ex.Message}", ex, _logger);
        }

        return entity.Id;
    }

    public async Task<List<long>> SaveAllAsync(IEnumerable<InArtist> entities)
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
            throw new RepositoryException("❌ SAVE ALL Artist : Could not persist.", ex, _logger);
        }
        
        return idList;
    }
}