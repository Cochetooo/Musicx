using System.Text;
using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.Genre;
using Musicx.Application.API.Persistence.Queries;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Genre;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Genre;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Infrastructure.API.Persistence.Builders;
using Musicx.Infrastructure.API.Persistence.Columns.Genre;
using Musicx.Infrastructure.API.Persistence.Connection;
using Musicx.Infrastructure.API.Persistence.Mappers;
using Musicx.Infrastructure.Shared.Exceptions;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Repositories.Genre;

internal sealed class GenreRepository(
    IDbConnectionProvider connection,
    SqlBuilder<InGenre> builder,
    ILoggerProvider loggerProvider) : IGenreRepository
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(GenreRepository));
    
    public async Task DeleteAsync(long id)
    {
        const string sql = $"DELETE FROM genres WHERE {GenreColumns.Id} = @id";

        var parameters = new List<NpgsqlParameter>
        {
            new("@id", id)
        };

        await connection.ExecuteTransactionAsync((sql, parameters));
    }

    public async Task DeleteAllAsync(IEnumerable<long> ids)
    {
        var stringIds = string.Join(",", ids);
        const string sql = $"DELETE FROM genres WHERE {GenreColumns.Id} IN (@ids)";
        
        var parameters = new List<NpgsqlParameter>
        {
            new("@Ids", stringIds)
        };
        
        await connection.ExecuteTransactionAsync((sql, parameters));
    }

    public async Task<OutGenre?> FindOneByIdAsync(long id, IJoinSpecification<InGenre>? genreQuerySpecification = null)
    {
        var sql = new StringBuilder(builder.BuildSelect(genreQuerySpecification));
        sql.Append($" WHERE g0.{GenreColumns.Id} = @id")
            .Append(builder.BuildGroupBy(genreQuerySpecification));
        
        var parameters = new List<NpgsqlParameter>
        {
            new("@id", id)
        };

        var result = await connection.FetchListDynamicAsync(sql.ToString(), parameters);

        return result
            .SingleOrDefault()?
            .FromDicoToGenre();
    }

    public async Task<OutGenericList<OutGenre>> FindAsync(
        IFindQuery<InGenre>? query,
        IJoinSpecification<InGenre>? joinSpec = null,
        OrderSpecification<InGenre>? orderSpec = null,
        PagingOptions? pagingOptions = null)
    {
        if (query is not GenreFindQuery typedQuery)
        {
            return new OutGenericList<OutGenre>();
        }
        
        var (sql, parameters) = builder.BuildFilteredQuery(typedQuery, joinSpec, orderSpec, pagingOptions, false);

        var result = await connection.FetchListDynamicAsync(sql, parameters);
        var total = await CountAsync(typedQuery, joinSpec);
        
        return new OutGenericList<OutGenre>
        {
            Items = result.Select(x => x.FromDicoToGenre()).ToList(),
            Total = total
        };
    }

    public async Task<List<OutGenre>> FindInAsync(IEnumerable<long> ids,
        IJoinSpecification<InGenre>? joinSpec = null,
        OrderSpecification<InGenre>? orderSpec = null)
    {
        var idList = ids.ToArray();
        if (0 == idList.Length)
        {
            return [];
        }
        
        var stringIds = string.Join(",", idList);
        var sql = builder.BuildSelect(joinSpec);
        sql += $" WHERE g0.{GenreColumns.Id} IN ({stringIds})" +
               builder.BuildGroupBy(joinSpec);

        if (orderSpec is not null)
        {
            sql += builder.BuildOrderBy(orderSpec);
        }
        
        var result = await connection.FetchListDynamicAsync(sql, []);
        
        return result
            .Select(x => x.FromDicoToGenre())
            .ToList();
    }

    public async Task<long> CountAsync(IFindQuery<InGenre>? query = null,
        IJoinSpecification<InGenre>? joinSpec = null)
    {
        var typedQuery = query as GenreFindQuery ?? new GenreFindQuery();
        var (sql, parameters) = builder.BuildFilteredQuery(typedQuery, joinSpec, null, null, true);
        
        await using var conn = (NpgsqlConnection)connection.CreateConnection();
        await conn.OpenAsync();
        await using var command = new NpgsqlCommand(sql, conn);
        command.Parameters.AddRange(parameters.ToArray());
        
        var result = await command.ExecuteScalarAsync();
        return result is null ? 0 : Convert.ToInt64(result);
    }

    public async Task<long> SaveAsync(InGenre entity)
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
            throw new RepositoryException("❌ SAVE Album : Could not persist.", ex, _logger);
        }
        
        return entity.Id;
    }

    public async Task<List<long>> SaveAllAsync(IEnumerable<InGenre> entities)
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
            throw new RepositoryException("❌ SAVE ALL Album : Could not persist.", ex, _logger);
        }
        
        return idList;
    }
}