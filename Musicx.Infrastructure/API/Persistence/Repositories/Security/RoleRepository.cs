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

internal sealed class RoleRepository(
    IDbConnectionProvider connection,
    SqlBuilder<InRole> builder,
    ILoggerProvider loggerProvider) : IRoleRepository
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(RoleRepository));

    public async Task DeleteAsync(long id)
    {
        const string sql = $"DELETE FROM roles WHERE {RoleColumns.Id} = @id";

        var parameters = new List<NpgsqlParameter>
        {
            new("@id", id)
        };

        await connection.ExecuteTransactionAsync((sql, parameters));
    }

    public async Task DeleteAllAsync(IEnumerable<long> ids)
    {
        var stringIds = string.Join(",", ids);
        const string sql = $"DELETE FROM roles WHERE {RoleColumns.Id} IN (@ids)";

        var parameters = new List<NpgsqlParameter>
        {
            new("@Ids", stringIds)
        };
        
        await connection.ExecuteTransactionAsync((sql, parameters));
    }

    public async Task<OutRole?> FindOneByIdAsync(long id, IJoinSpecification<InRole>? roleQuerySpecification = null)
    {
        var sql = new StringBuilder(builder.BuildSelect(roleQuerySpecification));
        sql.Append($" WHERE r0.{RoleColumns.Id} = @id")
            .Append(builder.BuildGroupBy(roleQuerySpecification));

        var parameters = new List<NpgsqlParameter>
        {
            new("@id", id)
        };
        
        var result = await connection.FetchListDynamicAsync(sql.ToString(), parameters);

        return result
            .SingleOrDefault()?
            .FromDicoToRole();
    }

    public async Task<OutGenericList<OutRole>> FindAsync(
        IFindQuery<InRole>? query,
        IJoinSpecification<InRole>? joinSpec = null,
        OrderSpecification<InRole>? orderSpec = null,
        PagingOptions? pagingOptions = null)
    {
        if (query is not RoleFindQuery typedQuery)
        {
            return new OutGenericList<OutRole>();
        }
        
        var (sql, parameters) = builder.BuildFilteredQuery(typedQuery, joinSpec, orderSpec, pagingOptions, false);
        var result = await connection.FetchListDynamicAsync(sql, parameters);
        var total = await CountAsync(typedQuery, joinSpec);
        
        return new OutGenericList<OutRole>
        {
            Items = result.Select(x => x.FromDicoToRole()).ToList(),
            Total = total
        };
    }

    public async Task<List<OutRole>> FindInAsync(IEnumerable<long> ids,
        IJoinSpecification<InRole>? joinSpec = null,
        OrderSpecification<InRole>? orderSpec = null)
    {
        var idList = ids.ToArray();
        if (0 == idList.Length)
        {
            return [];
        }

        var stringIds = string.Join(",", idList);
        var sql = builder.BuildSelect(joinSpec);
        sql += $" WHERE r0.{RoleColumns.Id} IN ({stringIds})" +
               builder.BuildGroupBy(joinSpec);
        
        if (orderSpec is not null)
        {
            sql += builder.BuildOrderBy(orderSpec);
        }

        var result = await connection.FetchListDynamicAsync(sql, []);

        return result
            .Select(x => x.FromDicoToRole())
            .ToList();
    }

    public async Task<long> CountAsync(IFindQuery<InRole>? query = null,
        IJoinSpecification<InRole>? joinSpec = null)
    {
        var typedQuery = query as RoleFindQuery ?? new RoleFindQuery();
        var (sql, parameters) = builder.BuildFilteredQuery(typedQuery, joinSpec, null, null, true);

        await using var conn = (NpgsqlConnection)connection.CreateConnection();
        await conn.OpenAsync();
        await using var command = new NpgsqlCommand(sql, conn);
        command.Parameters.AddRange(parameters.ToArray());
        
        var result = await command.ExecuteScalarAsync();
        return result is null ? 0 : Convert.ToInt64(result);
    }
    
    public async Task<long> SaveAsync(InRole entity)
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
            throw new RepositoryException("❌ SAVE Role : Could not persist.", ex, _logger);
        }
        
        return entity.Id;
    }

    public async Task<List<long>> SaveAllAsync(IEnumerable<InRole> entities)
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
            throw new RepositoryException("❌ SAVE ALL Role : Could not persist.", ex, _logger);
        }
        
        return idList;
    }
}