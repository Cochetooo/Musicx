using System.Linq.Expressions;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Persistence;
using Musicx.Application.Api.Interfaces.Specifications;
using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Infrastructure.API.Persistence.Builders;
using Musicx.Infrastructure.API.Persistence.Columns;
using Musicx.Infrastructure.API.Persistence.Mappers;
using Musicx.Infrastructure.Shared.Exceptions;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Repositories;

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

    public async Task<OutGenre?> FindByIdAsync(long id, IQuerySpecification<InGenre>? genreQuerySpecification = null)
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

    public async Task<List<OutGenre>> FindAsync(int skip = 0, int take = 100,
        IQuerySpecification<InGenre>? genreQuerySpecification = null, string? filter = null)
    {
        var sql = builder.BuildSelect(genreQuerySpecification);
        var parameters = new List<NpgsqlParameter>();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            sql += $" WHERE similarity(g0.{GenreColumns.Name}, @filter) > 0.4";
            parameters.Add(new NpgsqlParameter("@filter", filter));
        }
        
        sql += builder.BuildGroupBy(genreQuerySpecification);
        
        if (!string.IsNullOrWhiteSpace(filter))
        {
            sql += builder.BuildOrderBy($"similarity(g0.{GenreColumns.Name}, @filter) DESC");
        }
        else
        {
            sql += builder.BuildOrderBy($"g0.{GenreColumns.Name}");
        }
        
        sql += " OFFSET @skip LIMIT @take";
        
        parameters.Add(new NpgsqlParameter("@skip", skip));
        parameters.Add(new NpgsqlParameter("@take", take));

        var result = await connection.FetchListDynamicAsync(sql, parameters);

        return result
            .Select(x => x.FromDicoToGenre())
            .ToList();
    }

    public async Task<List<OutGenre>> FindIn(IEnumerable<long> ids, IQuerySpecification<InGenre>? genreQuerySpecification = null)
    {
        var idList = ids.ToArray();
        if (0 == idList.Length)
        {
            return [];
        }
        
        var stringIds = string.Join(",", idList);
        var sql = builder.BuildSelect(genreQuerySpecification);
        sql += $" WHERE g0.{GenreColumns.Id} IN ({stringIds})" +
               builder.BuildGroupBy(genreQuerySpecification) +
               $" ORDER BY g0.{GenreColumns.Name}";
        
        var result = await connection.FetchListDynamicAsync(sql, []);
        
        return result
            .Select(x => x.FromDicoToGenre())
            .ToList();
    }

    public async Task<long> GetCountAsync()
        => await connection.Count("genres");

    public async Task<long> SaveAsync(InGenre entity)
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
            throw new RepositoryException("❌ SAVE Album : Could not persist.", ex, _logger);
        }
        
        return entity.Id;
    }

    public async Task<List<long>> SaveAllAsync(IEnumerable<InGenre> entities)
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
            throw new RepositoryException("❌ SAVE ALL Album : Could not persist.", ex, _logger);
        }
        
        return idList;
    }
}