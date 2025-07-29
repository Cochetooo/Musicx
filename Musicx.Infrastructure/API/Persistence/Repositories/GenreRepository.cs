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
using Musicx.Infrastructure.API.Persistence.Columns;
using Musicx.Infrastructure.API.Persistence.Mappers;
using Musicx.Infrastructure.Shared.Exceptions;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Repositories;

internal sealed class GenreRepository(
    IDbConnectionProvider connection,
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
        var sql = new StringBuilder(Select(genreQuerySpecification));
        sql.Append($" WHERE g0.{GenreColumns.Id} = @id")
            .Append(GroupBy(genreQuerySpecification));
        
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
        var sql = Select(genreQuerySpecification);
        var parameters = new List<NpgsqlParameter>();

        if (filter is not null)
        {
            sql += $" WHERE g0.{GenreColumns.Name} ILIKE @filter";
            parameters.Add(new NpgsqlParameter("@filter", $"%{filter}%"));
        }
        
        sql += GroupBy(genreQuerySpecification);
        sql += $" ORDER BY g0.{GenreColumns.Name} OFFSET @skip LIMIT @take";
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
        var sql = Select(genreQuerySpecification);
        sql += $" WHERE g0.{GenreColumns.Id} IN ({stringIds})" +
               GroupBy(genreQuerySpecification) +
               $" ORDER BY g0.{GenreColumns.Name}";
        
        var result = await connection.FetchListDynamicAsync(sql, []);
        
        return result
            .Select(x => x.FromDicoToGenre())
            .ToList();
    }

    public async Task<int> GetCountAsync()
        => await connection.Count("genres");

    public async Task<long> SaveAsync(InGenre entity)
    {
        /*await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            if (0 == entity.Id)
            {
                _logger.LogDebug($"📄 SQL : INSERT INTO genres (name, color, parent_ids) " +
                                 $"VALUES ('{entity.Name}', '{entity.Color}', '{string.Join(",", entity.Parents.Select(p => p.Id))}')");
                
                entity.CreatedAt = DateTime.Now;
                entity.UpdatedAt = DateTime.Now;

                foreach (var parent in entity.Parents)
                {
                    context.Attach(parent);
                }

                foreach (var child in entity.Children)
                {
                    context.Attach(child);
                }
                
                context.Genres.Add(entity);
            }
            else
            {
                _logger.LogDebug($"📄 SQL : UPDATE genres SET Name={entity.Name} " +
                                 $"WHERE Id = {entity.Id}");
                
                entity.UpdatedAt = DateTime.Now;
                context.Genres.Update(entity);
                
                context.Entry(entity).Property(x => x.CreatedAt).IsModified = false;
                
                context.Entry(entity).Collection(e => e.Parents).IsModified = true;
                context.Entry(entity).Collection(e => e.Children).IsModified = true;
            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return entity.Id;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new RepositoryException("❌ SAVE Album : Could not persist.", ex, _logger);
        }*/
        return 1;
    }

    public async Task<List<long>> SaveAllAsync(IEnumerable<InGenre> entities)
    {
        _logger.LogDebug("📄 SAVE ALL Genre");
        return [];

        /*await using var transaction = await context.Database.BeginTransactionAsync();

        var ids = new List<long>();

        try
        {
            foreach (var genre in entities)
            {
                if (0 == genre.Id)
                {
                    context.Genres.Add(genre);
                }
                else
                {
                    context.Genres.Update(genre);
                }

                ids.Add(genre.Id);
            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return ids;
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"❌ SAVE ALL Genre : Could not persist.", ex);
            await transaction.RollbackAsync();
            throw;
        }*/
    }

    private static string Select(IQuerySpecification<InGenre>? querySpecification = null)
    {
        if (querySpecification is not GenreQuerySpecification genreQuerySpecification)
        {
            return "SELECT g0.* FROM genres g0";
        }

        var selects = new List<string> { "g0.*" };
        var joins = new List<string>();

        if (genreQuerySpecification.IncludeChildren)
        {
            selects.Add($"json_agg(cg.*) FILTER (WHERE cg.{GenreColumns.Id} IS NOT NULL) AS children");
            joins.Add($"LEFT JOIN childrengenre_parentgenre cgpg ON g0.{GenreColumns.Id} = cgpg.{ChildrenGenreParentGenreColumns.ParentId}");
            joins.Add($"LEFT JOIN genre cg ON cgpg.{ChildrenGenreParentGenreColumns.ChildId} = cg.{GenreColumns.Id}");
        }
        
        if (genreQuerySpecification.IncludeParents)
        {
            selects.Add($"json_agg(pg.*) FILTER (WHERE pg.{GenreColumns.Id} IS NOT NULL) AS parents");
            joins.Add($"LEFT JOIN childrengenre_parentgenre pgcg ON g0.{GenreColumns.Id} = pgcg.{ChildrenGenreParentGenreColumns.ChildId}");
            joins.Add($"LEFT JOIN genre pg ON pgcg.{ChildrenGenreParentGenreColumns.ParentId} = pg.{GenreColumns.Id}");
        }

        return $"SELECT {string.Join(", ", selects)} FROM genres g0 {string.Join(" ", joins)}";
    }

    private static string GroupBy(IQuerySpecification<InGenre>? querySpecification = null)
    {
        return $" GROUP BY g0.{GenreColumns.Id}";
    }
}