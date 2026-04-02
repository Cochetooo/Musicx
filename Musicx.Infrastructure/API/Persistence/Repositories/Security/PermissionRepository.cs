using System.Text;
using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.Security;
using Musicx.Application.API.Persistence.Queries;
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

    public async Task<OutGenericList<OutPermission>> FindAsync(
        IFindQuery<InPermission>? query,
        IJoinSpecification<InPermission>? joinSpec = null,
        OrderSpecification<InPermission>? orderSpec = null,
        PagingOptions? pagingOptions = null)
    {
        if (query is not PermissionFindQuery typedQuery)
        {
            return new OutGenericList<OutPermission>();
        }
        
        var (sql, parameters) = builder.BuildFilteredQuery(typedQuery, joinSpec, orderSpec, pagingOptions, false);
        var result = await connection.FetchListDynamicAsync(sql, parameters);
        var total = await CountAsync(typedQuery, joinSpec);
        
        return new OutGenericList<OutPermission>
        {
            Items = result.Select(x => x.FromDicoToPermission()).ToList(),
            Total = total
        };
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

    public async Task<long> CountAsync(IFindQuery<InPermission>? query = null,
        IJoinSpecification<InPermission>? joinSpec = null)
    {
        var typedQuery = query as PermissionFindQuery ?? new PermissionFindQuery();
        var (sql, parameters) = builder.BuildFilteredQuery(typedQuery, joinSpec, null, null, true);
        
        await using var conn = (NpgsqlConnection)connection.CreateConnection();
        await conn.OpenAsync();
        await using var command = new NpgsqlCommand(sql, conn);
        command.Parameters.AddRange(parameters.ToArray());
        
        var result = await command.ExecuteScalarAsync();
        return result is null ? 0 : Convert.ToInt64(result);
    }
    
    public async Task<long> SaveAsync(InPermission entity)
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
                await builder.ExecuteUpsert(entity, conn, transaction);
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