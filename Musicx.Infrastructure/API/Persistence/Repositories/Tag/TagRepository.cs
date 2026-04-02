using System.Text;
using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Persistence.Repositories.Tag;
using Musicx.Application.API.Persistence.Queries;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Tag;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Infrastructure.API.Persistence.Builders;
using Musicx.Infrastructure.API.Persistence.Columns.Tag;
using Musicx.Infrastructure.API.Persistence.Connection;
using Musicx.Infrastructure.API.Persistence.Mappers;
using Musicx.Infrastructure.Shared.Exceptions;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Repositories.Tag;

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

    public async Task<OutTag?> FindOneByIdAsync(long id, IJoinSpecification<InTag>? songQuerySpecification = null)
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

    public async Task<OutGenericList<OutTag>> FindAsync(
        IFindQuery<InTag>? query,
        IJoinSpecification<InTag>? joinSpec = null,
        OrderSpecification<InTag>? orderSpec = null,
        PagingOptions? pagingOptions = null)
    {
        if (query is not TagFindQuery typedQuery)
        {
            return new OutGenericList<OutTag>();
        }
        
        var sql = builder.BuildSelect();

        var parameters = new List<NpgsqlParameter>();

        if (!string.IsNullOrWhiteSpace(query?.RawSearch?.Value))
        {
            builder.Filter(
                sql: ref sql, 
                column: $"t0.{TagColumns.Name}", 
                filter: query.RawSearch.Value!,
                parameters: parameters, 
                filterExact: query!.Search?.Exact ?? false, 
                filterSimilitude: query!.Search?.Similarity ?? 0.4
            );
        }
        
        sql += builder.BuildGroupBy();

        if (orderSpec is not null)
        {
            sql += builder.BuildOrderBy(orderSpec);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(query?.RawSearch?.Value))
            {
                sql += $" ORDER BY similarity(t0.{TagColumns.Name}, @filter) DESC";
            }
            else
            {
                sql += $" ORDER BY t0.{TagColumns.Name}";
            }
        }
        
        sql +=  " OFFSET @skip LIMIT @take";
        
        parameters.Add(new NpgsqlParameter("@skip", pagingOptions?.Skip ?? 0));
        parameters.Add(new NpgsqlParameter("@take", pagingOptions?.Take ?? 200));
        
        var result = await connection.FetchListDynamicAsync(sql, parameters);
        
        return result
            .Select(x => x.FromDicoToTag())
            .ToList();
    }

    public async Task<List<OutTag>> FindInAsync(IEnumerable<long> ids,
        IJoinSpecification<InTag>? joinSpec = null,
        OrderSpecification<InTag>? orderSpec = null)
    {
        var idList = ids.ToArray();
        if (0 == idList.Length)
        {
            return [];
        }

        var stringIds = string.Join(",", idList);
        var sql = builder.BuildSelect();
        sql += $" WHERE t0.{TagColumns.Id} IN ({stringIds})" +
               builder.BuildGroupBy();

        if (orderSpec is not null)
        {
            sql += builder.BuildOrderBy(orderSpec);
        }

        var result = await connection.FetchListDynamicAsync(sql, []);

        return result
            .Select(x => x.FromDicoToTag())
            .ToList();
    }

    public async Task<long> CountAsync(IFindQuery<InTag>? query = null,
        IJoinSpecification<InTag>? joinSpec = null)
        => await connection.Count("tags");

    public async Task<long> SaveAsync(InTag entity)
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

    public async Task<List<long>> SaveAllAsync(IEnumerable<InTag> entities)
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