using System.Text;
using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.Security;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Security;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Infrastructure.API.Persistence.Builders;
using Musicx.Infrastructure.API.Persistence.Columns.Security;
using Musicx.Infrastructure.API.Persistence.Connection;
using Musicx.Infrastructure.API.Persistence.Mappers;
using Musicx.Infrastructure.Shared.Exceptions;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Repositories.Security;

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

    public async Task<OutPermission?> FindOneByIdAsync(long id, IJoinSpecification<InPermission>? permissionQuerySpecification = null)
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

    public async Task<List<OutPermission>> FindAllAsync(
        bool? filterExact = null, double? filterSimilitude = null, string? filter = null,
        IJoinSpecification<InPermission>? joinSpec = null,
        OrderSpecification<InPermission>? orderSpec = null,
        PagingOptions? pagingOptions = null)
    {
        var sql = builder.BuildSelect(joinSpec);

        var parameters = new List<NpgsqlParameter>();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            builder.Filter(
                sql: ref sql, 
                column: $"p0.{PermissionColumns.Name}", 
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
                sql += $" ORDER BY similarity(p0.{PermissionColumns.Name}, @filter) DESC";
            }
            else
            {
                sql += $" ORDER BY al0.{PermissionColumns.Name}";
            }
        }

        sql += " OFFSET @skip LIMIT @take";
        
        parameters.Add(new NpgsqlParameter("@skip", pagingOptions?.Skip ?? 0));
        parameters.Add(new NpgsqlParameter("@take", pagingOptions?.Take ?? 200));
        
        var result = await connection.FetchListDynamicAsync(sql, parameters);
        
        return result
            .Select(x => x.FromDicoToPermission())
            .ToList();
    }

    public async Task<List<OutPermission>> FindInAsync(IEnumerable<long> ids,
        IJoinSpecification<InPermission>? joinSpec = null,
        OrderSpecification<InPermission>? orderSpec = null)
    {
        var idList = ids.ToArray();
        if (0 == idList.Length)
        {
            return [];
        }

        var stringIds = string.Join(",", idList);
        var sql = builder.BuildSelect(joinSpec);
        sql += $" WHERE p0.{PermissionColumns.Id} IN ({stringIds})" +
               builder.BuildGroupBy(joinSpec);

        if (orderSpec is not null)
        {
            sql += builder.BuildOrderBy(orderSpec);
        }

        var result = await connection.FetchListDynamicAsync(sql, []);

        return result
            .Select(x => x.FromDicoToPermission())
            .ToList();
    }

    public async Task<long> GetCountAsync()
        => await connection.Count("permissions");
    
    public async Task<long> SaveAsync(InPermission entity)
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
            throw new RepositoryException("❌ SAVE Permission : Could not persist.", ex, _logger);
        }
        
        return entity.Id;
    }

    public async Task<List<long>> SaveAllAsync(IEnumerable<InPermission> entities)
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
            throw new RepositoryException("❌ SAVE ALL Permission : Could not persist.", ex, _logger);
        }
        
        return idList;
    }
}