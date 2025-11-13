using System.Text;
using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Persistence;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Infrastructure.API.Persistence.Builders;
using Musicx.Infrastructure.API.Persistence.Columns;
using Musicx.Infrastructure.API.Persistence.Mappers;
using Musicx.Infrastructure.Shared.Exceptions;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Repositories;

internal sealed class TagRepository(
    IDbConnectionProvider connection,
    SqlBuilder<InTag> builder,
    ILoggerProvider loggerProvider) : ITagRepository
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(TagRepository));
    
    public async Task DeleteAsync(long id)
    {
        const string sql = $"DELETE FROM tags WHERE {TagColumns.Id} = @id";

        var parameters = new List<NpgsqlParameter>
        {
            new("@id", id)
        };

        await connection.ExecuteTransactionAsync((sql, parameters));
    }

    public async Task DeleteAllAsync(IEnumerable<long> ids)
    {
        var stringIds = string.Join(",", ids);
        const string sql = $"DELETE FROM tags WHERE {TagColumns.Id} IN (@ids)";

        var parameters = new List<NpgsqlParameter>
        {
            new("@Ids", stringIds)
        };
        
        await connection.ExecuteTransactionAsync((sql, parameters));
    }

    public async Task<OutTag?> FindByIdAsync(long id, IQuerySpecification<InTag>? songQuerySpecification = null)
    {
        var sql = new StringBuilder(builder.BuildSelect());
        sql.Append($" WHERE t0.{TagColumns.Id} = @id")
            .Append(builder.BuildGroupBy());

        var parameters = new List<NpgsqlParameter>
        {
            new("@id", id)
        };
        
        var result = await connection.FetchListDynamicAsync(sql.ToString(), parameters);

        return result
            .SingleOrDefault()?
            .FromDicoToTag();
    }

    public async Task<List<OutTag>> FindAsync(long skip = 0, long take = 100, bool? filterExact = null, double? filterSimilitude = 0.4,
        string? filter = null, string? order = null, IQuerySpecification<InTag>? songQuerySpecification = null)
    {
        var sql = builder.BuildSelect();

        var parameters = new List<NpgsqlParameter>();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            builder.Filter(
                sql: ref sql, 
                column: $"t0.{TagColumns.Name}", 
                filter: filter,
                parameters: parameters, 
                filterExact: filterExact, 
                filterSimilitude: filterSimilitude
            );
        }
        
        sql += builder.BuildGroupBy();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            sql += $" ORDER BY similarity(t0.{TagColumns.Name}, @filter) DESC";
        }
        else
        {
            sql += $" ORDER BY t0.{TagColumns.Name}";
        }

        sql += " OFFSET @skip LIMIT @take";
        
        parameters.Add(new NpgsqlParameter("@skip", skip));
        parameters.Add(new NpgsqlParameter("@take", take));
        
        var result = await connection.FetchListDynamicAsync(sql, parameters);
        
        return result
            .Select(x => x.FromDicoToTag())
            .ToList();
    }

    public async Task<List<OutTag>> FindIn(IEnumerable<long> ids, IQuerySpecification<InTag>? songQuerySpecification = null)
    {
        var idList = ids.ToArray();
        if (0 == idList.Length)
        {
            return [];
        }

        var stringIds = string.Join(",", idList);
        var sql = builder.BuildSelect();
        sql += $" WHERE t0.{TagColumns.Id} IN ({stringIds})" +
               builder.BuildGroupBy() +
               builder.BuildOrderBy(
                   $"t0.{TagColumns.Name}");

        var result = await connection.FetchListDynamicAsync(sql, []);

        return result
            .Select(x => x.FromDicoToTag())
            .ToList();
    }

    public async Task<long> GetCountAsync()
        => await connection.Count("tags");

    public async Task<long> SaveAsync(InTag entity)
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
            throw new RepositoryException("❌ SAVE Role : Could not persist.", ex, _logger);
        }
        
        return entity.Id;
    }

    public async Task<List<long>> SaveAllAsync(IEnumerable<InTag> entities)
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
            throw new RepositoryException("❌ SAVE ALL Role : Could not persist.", ex, _logger);
        }
        
        return idList;
    }
}