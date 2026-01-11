using System.Text;
using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.Security;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Security;
using Musicx.Contracts.Dto.Responses;
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

    public async Task<OutRole?> FindByIdAsync(long id, IJoinSpecification<InRole>? roleQuerySpecification = null)
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

    public async Task<List<OutRole>> FindAsync(
        bool? filterExact = false, double? filterSimilitude = 0.4, string? filter = null,
        IJoinSpecification<InRole>? joinSpec = null,
        OrderSpecification<InRole>? orderSpec = null,
        PagingOptions? pagingOptions = null)
    {
        var sql = builder.BuildSelect(joinSpec);

        var parameters = new List<NpgsqlParameter>();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            builder.Filter(
                sql: ref sql, 
                column: $"r0.{RoleColumns.Name}", 
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
                sql += $" ORDER BY similarity(r0.{RoleColumns.Name}, @filter) DESC";
            }
            else
            {
                sql += $" ORDER BY r0.{RoleColumns.Name}";
            }
        }
        
        sql +=  " OFFSET @skip LIMIT @take";
        
        parameters.Add(new NpgsqlParameter("@skip", pagingOptions?.Skip ?? 0));
        parameters.Add(new NpgsqlParameter("@take", pagingOptions?.Take ?? 200));
        
        var result = await connection.FetchListDynamicAsync(sql, parameters);
        
        return result
            .Select(x => x.FromDicoToRole())
            .ToList();
    }

    public async Task<List<OutRole>> FindIn(IEnumerable<long> ids,
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

    public async Task<long> GetCountAsync()
        => await connection.Count("roles");
    
    public async Task<long> SaveAsync(InRole entity)
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

    public async Task<List<long>> SaveAllAsync(IEnumerable<InRole> entities)
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