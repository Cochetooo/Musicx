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

internal sealed class PermissionRepository(
    IDbConnectionProvider connection,
    SqlBuilder<InPermission> builder,
    ILoggerProvider loggerProvider) : IPermissionRepository
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(PermissionRepository));

    public async Task DeleteAsync(long id)
    {
        const string sql = $"DELETE FROM permissions WHERE {PermissionColumns.Id} = @id";

        var parameters = new List<NpgsqlParameter>
        {
            new("@id", id)
        };

        await connection.ExecuteTransactionAsync((sql, parameters));
    }

    public async Task DeleteAllAsync(IEnumerable<long> ids)
    {
        var stringIds = string.Join(",", ids);
        const string sql = $"DELETE FROM permissions WHERE {PermissionColumns.Id} IN (@ids)";

        var parameters = new List<NpgsqlParameter>
        {
            new("@Ids", stringIds)
        };
        
        await connection.ExecuteTransactionAsync((sql, parameters));
    }

    public async Task<OutPermission?> FindByIdAsync(long id, IQuerySpecification<InPermission>? permissionQuerySpecification = null)
    {
        var sql = new StringBuilder(builder.BuildSelect(permissionQuerySpecification));
        sql.Append($" WHERE p0.{PermissionColumns.Id} = @id")
            .Append(builder.BuildGroupBy(permissionQuerySpecification));

        var parameters = new List<NpgsqlParameter>
        {
            new("@id", id)
        };
        
        var result = await connection.FetchListDynamicAsync(sql.ToString(), parameters);

        return result
            .SingleOrDefault()?
            .FromDicoToPermission();
    }

    public async Task<List<OutPermission>> FindAsync(int skip = 0, int take = 100,
        IQuerySpecification<InPermission>? permissionQuerySpecification = null,
        string? filter = null)
    {
        var sql = builder.BuildSelect(permissionQuerySpecification);

        var parameters = new List<NpgsqlParameter>();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            sql += $" WHERE similarity(p0.{PermissionColumns.Name}, @filter) > 0.4";
            parameters.Add(new NpgsqlParameter("@filter", filter));
        }
        
        sql += builder.BuildGroupBy(permissionQuerySpecification);

        if (!string.IsNullOrWhiteSpace(filter))
        {
            sql += $" ORDER BY similarity(p0.{PermissionColumns.Name}, @filter) DESC";
        }
        else
        {
            sql += $" ORDER BY p0.{PermissionColumns.Name}";
        }

        sql += " OFFSET @skip LIMIT @take";
        
        parameters.Add(new NpgsqlParameter("@skip", skip));
        parameters.Add(new NpgsqlParameter("@take", take));
        
        var result = await connection.FetchListDynamicAsync(sql, parameters);
        
        return result
            .Select(x => x.FromDicoToPermission())
            .ToList();
    }

    public async Task<List<OutPermission>> FindIn(IEnumerable<long> ids,
        IQuerySpecification<InPermission>? permissionQuerySpecification = null)
    {
        var idList = ids.ToArray();
        if (0 == idList.Length)
        {
            return [];
        }

        var stringIds = string.Join(",", idList);
        var sql = builder.BuildSelect(permissionQuerySpecification);
        sql += $" WHERE p0.{PermissionColumns.Id} IN ({stringIds})" +
               builder.BuildGroupBy(permissionQuerySpecification) +
               builder.BuildOrderBy(
                   $"p0.{PermissionColumns.Name}");

        var result = await connection.FetchListDynamicAsync(sql, []);

        return result
            .Select(x => x.FromDicoToPermission())
            .ToList();
    }

    public async Task<long> GetCountAsync()
        => await connection.Count("permissions");
    
    public async Task<long> SaveAsync(InPermission entity)
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
            throw new RepositoryException("❌ SAVE Permission : Could not persist.", ex, _logger);
        }
        
        return entity.Id;
    }

    public async Task<List<long>> SaveAllAsync(IEnumerable<InPermission> entities)
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
            throw new RepositoryException("❌ SAVE ALL Permission : Could not persist.", ex, _logger);
        }
        
        return idList;
    }
}