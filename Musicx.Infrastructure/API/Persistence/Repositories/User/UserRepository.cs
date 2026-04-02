using System.Text;
using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.User;
using Musicx.Application.Api.Models.Auth;
using Musicx.Application.API.Persistence.Queries;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Contracts.Dto.Responses.User;
using Musicx.Infrastructure.API.Persistence.Builders;
using Musicx.Infrastructure.API.Persistence.Columns.User;
using Musicx.Infrastructure.API.Persistence.Connection;
using Musicx.Infrastructure.API.Persistence.Mappers;
using Musicx.Infrastructure.Shared.Exceptions;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Repositories.User;

internal sealed class UserRepository(
    IDbConnectionProvider connection,
    SqlBuilder<InUser> builder,
    ILoggerProvider loggerProvider) : IUserRepository
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(UserRepository));

    public async Task DeleteAsync(long id)
    {
        const string sql = $"DELETE FROM users WHERE {UserColumns.Id} = @id";

        var parameters = new List<NpgsqlParameter>
        {
            new("@id", id)
        };

        await connection.ExecuteTransactionAsync((sql, parameters));
    }

    public async Task DeleteAllAsync(IEnumerable<long> ids)
    {
        var stringIds = string.Join(",", ids);
        const string sql = $"DELETE FROM users WHERE {UserColumns.Id} IN (@ids)";

        var parameters = new List<NpgsqlParameter>
        {
            new("@Ids", stringIds)
        };
        
        await connection.ExecuteTransactionAsync((sql, parameters));
    }

    public async Task<OutUser?> FindOneByIdAsync(long id, IJoinSpecification<InUser>? userQuerySpecification = null)
    {
        var sql = new StringBuilder(builder.BuildSelect(userQuerySpecification));
        sql.Append($" WHERE u0.{UserColumns.Id} = @id")
            .Append(builder.BuildGroupBy(userQuerySpecification));

        var parameters = new List<NpgsqlParameter>
        {
            new("@id", id)
        };
        
        var result = await connection.FetchListDynamicAsync(sql.ToString(), parameters);

        return result
            .SingleOrDefault()?
            .FromDicoToUser();
    }
    
    public async Task<OutUser?> FindByEmailAsync(string email, 
        IJoinSpecification<InUser>? joinSpec = null)
    {
        var sql = new StringBuilder(builder.BuildSelect(joinSpec));
        sql.Append($" WHERE u0.{UserColumns.Email} = @email")
            .Append(builder.BuildGroupBy(joinSpec));

        var parameters = new List<NpgsqlParameter>
        {
            new("@email", email)
        };
        
        var result = await connection.FetchListDynamicAsync(sql.ToString(), parameters);

        return result
            .SingleOrDefault()?
            .FromDicoToUser();
    }
    
    public async Task<OutUserAuth?> FindAuthByEmailAsync(string email,
        IJoinSpecification<InUser>? joinSpec = null)
    {
        var sql = new StringBuilder(builder.BuildSelect(joinSpec));
        sql.Append($" WHERE u0.{UserColumns.Email} = @email")
            .Append(builder.BuildGroupBy(joinSpec));

        var parameters = new List<NpgsqlParameter>
        {
            new("@email", email)
        };

        var result = await connection.FetchListDynamicAsync(sql.ToString(), parameters);

        return result
            .SingleOrDefault()?
            .FromDicoToUserAuth();
    }

    public async Task<OutGenericList<OutUser>> FindAsync(
        IFindQuery<InUser>? query,
        IJoinSpecification<InUser>? joinSpec = null,
        OrderSpecification<InUser>? orderSpec = null,
        PagingOptions? pagingOptions = null)
    {
        if (query is not UserFindQuery typedQuery)
        {
            return new OutGenericList<OutUser>();
        }
        
        var (sql, parameters) = builder.BuildFilteredQuery(typedQuery, joinSpec, orderSpec, pagingOptions, false);
        var result = await connection.FetchListDynamicAsync(sql, parameters);
        var total = await CountAsync(typedQuery, joinSpec);
        
        return new OutGenericList<OutUser>
        {
            Items = result.Select(x => x.FromDicoToUser()).ToList(),
            Total = total
        };
    }

    public async Task<List<OutUser>> FindInAsync(IEnumerable<long> ids,
        IJoinSpecification<InUser>? joinSpec = null,
        OrderSpecification<InUser>? orderSpec = null)
    {
        var idList = ids.ToArray();
        if (0 == idList.Length)
        {
            return [];
        }

        var stringIds = string.Join(",", idList);
        var sql = builder.BuildSelect(joinSpec);
        sql += $" WHERE u0.{UserColumns.Id} IN ({stringIds})" +
               builder.BuildGroupBy(joinSpec);

        if (orderSpec is not null)
        {
            sql += builder.BuildOrderBy(orderSpec);
        }

        var result = await connection.FetchListDynamicAsync(sql, []);

        return result
            .Select(x => x.FromDicoToUser())
            .ToList();
    }

    public async Task<long> CountAsync(
        IFindQuery<InUser>? query = null,
        IJoinSpecification<InUser>? spec = null)
    {
        var typedQuery = query as UserFindQuery ?? new UserFindQuery();
        var (sql, parameters) = builder.BuildFilteredQuery(typedQuery, spec, null, null, true);
        
        await using var conn = (NpgsqlConnection)connection.CreateConnection();
        await conn.OpenAsync();
        await using var command = new NpgsqlCommand(sql, conn);
        command.Parameters.AddRange(parameters.ToArray());
        
        var result = await command.ExecuteScalarAsync();
        return result is null ? 0 : Convert.ToInt64(result);
    }
    
    public async Task<long> SaveAsync(InUser entity)
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
            throw new RepositoryException("❌ SAVE User : Could not persist.", ex, _logger);
        }
        
        return entity.Id;
    }

    public async Task<List<long>> SaveAllAsync(IEnumerable<InUser> entities)
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
            throw new RepositoryException("❌ SAVE ALL User : Could not persist.", ex, _logger);
        }
        
        return idList;
    }
}