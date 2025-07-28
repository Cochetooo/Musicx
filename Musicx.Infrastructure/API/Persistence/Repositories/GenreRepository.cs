using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Persistence;
using Musicx.Application.Api.Interfaces.Specifications;
using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Application.Shared.Interfaces.Persistence;

using Musicx.Infrastructure.Shared.Exceptions;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Repositories;

internal sealed class GenreRepository(
    ApiDbContext context,
    ILoggerProvider loggerProvider) : IGenreRepository
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(GenreRepository));
    
    public async Task DeleteAsync(long id)
    {
        _logger.LogDebug($"📄 SQL : DELETE FROM genres WHERE id = {id}");
        
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var genre = await context.Genres.FindAsync(id);

            if (null != genre)
            {
                context.Genres.Remove(genre);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new RepositoryException($"❌ Could not delete id {id}", ex, _logger);
        }
    }

    public async Task DeleteAllAsync(IEnumerable<long> ids)
    {
        var stringIds = string.Join(",", ids);

        await using var transaction = await context.Database.BeginTransactionAsync();
        
        try
        {
            var deleteAllSql = "DELETE FROM \"Genres\" WHERE \"Id\" IN (@ids)";

            var parameters = new List<NpgsqlParameter>
            {
                new("@Ids", stringIds)
            };

            _logger.LogDebug(SqlDebugHelper.InterpolateQuery(deleteAllSql, parameters));
            
            await context.Database.ExecuteSqlRawAsync(deleteAllSql, parameters.Cast<object>().ToArray());
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new RepositoryException($"📜❌ Could not delete ids {stringIds}", ex, _logger);
        }
    }

    public async Task<Genre?> FindByIdAsync(long id, IQuerySpecification<Genre>? genreQuerySpecification = null)
    {
        _logger.LogDebug($"📄 SQL : SELECT * FROM genres WHERE id = {id}");
        
        var genreSet = context.Genres
            .AsQueryable();
        
        genreSet = GetIncludes(genreSet, genreQuerySpecification);
        
        return await genreSet
            .AsNoTracking()
            .OrderBy(g => g.Name)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Genre>> FindAsync(int skip = 0, int take = 100, Expression<Func<Genre, bool>>? filter = null,
        IQuerySpecification<Genre>? genreQuerySpecification = null)
    {
        _logger.LogDebug("📄 SQL : SELECT * FROM genres");
        
        var genreSet = context.Genres
            .AsQueryable();
        
        genreSet = GetIncludes(genreSet, genreQuerySpecification);

        if (null != filter)
        {
            genreSet = genreSet.Where(filter);
        }
        
        return await genreSet
            .AsNoTracking()
            .Skip(skip)
            .Take(take)
            .OrderBy(g => g.Name)
            .ToListAsync();
    }

    public async Task<List<Genre>> FindIn(IEnumerable<long> ids, IQuerySpecification<Genre>? genreQuerySpecification = null)
    {
        _logger.LogDebug("📄 SQL : SELECT * FROM genres WHERE id IN ({Ids})", string.Join(",", ids));

        var enumerable = ids as long[] ?? ids.ToArray();
        
        if (0 == enumerable.Length)
        {
            _logger.LogDebug("ℹ️ FIND IN Genre : No entry found.");
            return [];
        }
        
        var genreSet = context.Genres
            .AsQueryable();
        
        genreSet = GetIncludes(genreSet, genreQuerySpecification);
        
        return await genreSet
            .Where(s => enumerable.Contains(s.Id))
            .AsNoTracking()
            .OrderBy(g => g.Name)
            .ToListAsync();
    }

    public async Task<int> GetCountAsync()
    {
        return await context.Genres.CountAsync();
    }

    public async Task<long> SaveAsync(Genre entity)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

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
        }
    }

    public async Task<List<long>> SaveAllAsync(IEnumerable<Genre> entities)
    {
        _logger.LogDebug("📄 SAVE ALL Genre");
        
        await using var transaction = await context.Database.BeginTransactionAsync();

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
        }
    }

    private IQueryable<Genre> GetIncludes(IQueryable<Genre> query, IQuerySpecification<Genre>? querySpecification = null)
    {
        if (querySpecification is not GenreQuerySpecification genreQuerySpecification)
            return query;
        
        if (genreQuerySpecification.IncludeChildren)
        {
            query = query.Include(s => s.Children);
        }

        if (genreQuerySpecification.IncludeParents)
        {
            query = query.Include(s => s.Parents);
        }

        return query;
    }
}