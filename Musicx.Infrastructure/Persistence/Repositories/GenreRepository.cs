using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Application.Desktop.Interfaces.Persistence;
using Musicx.Application.Desktop.Specifications;
using Musicx.Domain.Models;

namespace Musicx.Infrastructure.Persistence.Repositories;

public class GenreRepository(
    AppDbContext context,
    IGenreCache genreCache,
    ILoggerFactory loggerFactory) : IGenreRepository
{
    private readonly ILogger<GenreRepository> _logger = loggerFactory.CreateLogger<GenreRepository>();
    
    public async Task DeleteAsync(long id)
    {
        _logger.Db($"📄 Delete Genre : {id}");
        
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
            _logger.Fatal($"❌ Could not delete id {id}", ex);
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Genre?> FindByIdAsync(long id, IQuerySpecification<Genre>? genreQuerySpecification = null)
    {
        _logger.Db($"📄 Find By Id Genre : {id}");
        
        var genreSet = context.Genres;
        GetIncludes(genreSet, genreQuerySpecification);
        
        return await genreSet
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Genre>> FindAsync(int skip = 0, int take = 100, Expression<Func<Genre, bool>>? filter = null,
        IQuerySpecification<Genre>? genreQuerySpecification = null)
    {
        _logger.Db($"📄 Find Genre");
        
        var genreSet = context.Genres;
        
        GetIncludes(genreSet, genreQuerySpecification);
        var query = genreSet.AsQueryable();

        if (null != filter)
        {
            query = query.Where(filter);
        }
        
        return await query
            .AsNoTracking()
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<List<Genre>> FindIn(IEnumerable<long> ids, IQuerySpecification<Genre>? genreQuerySpecification = null)
    {
        _logger.Db("📄 Find In Genre");

        var enumerable = ids as long[] ?? ids.ToArray();
        
        if (!enumerable.Any())
        {
            _logger.Debug("ℹ️ Empty List in Find In");
            return [];
        }
        
        var genreSet = context.Genres;
        GetIncludes(genreSet, genreQuerySpecification);
        
        return await genreSet
            .Where(s => enumerable.Contains(s.Id))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<int> GetCountAsync()
    {
        return await context.Genres.CountAsync();
    }

    public async Task<long> SaveAsync(Genre entity)
    {
        _logger.Db("📄 Save Genre");
        
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            if (0 == entity.Id)
            {
                context.Genres.Add(entity);
            }
            else
            {
                context.Genres.Update(entity);
            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return entity.Id;
        }
        catch (Exception ex)
        {
            _logger.Fatal("❌ Failed saving.", ex);
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<long>> SaveAllAsync(IEnumerable<Genre> entities)
    {
        _logger.Db("📄 SaveAll Genre");
        
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
            _logger.Fatal($"❌ Failed saving", ex);
            await transaction.RollbackAsync();
            throw;
        }
    }

    private void GetIncludes(in DbSet<Genre> genreSet, IQuerySpecification<Genre>? querySpecification = null)
    {
        if (null == querySpecification)
        {
            return;
        }
        
        var genreQuerySpecification = (GenreQuerySpecification)querySpecification;

        if (genreQuerySpecification.IncludeChildren)
        {
            genreSet.Include(g => g.Children);
        }

        if (genreQuerySpecification.IncludeParents)
        {
            genreSet.Include(g => g.Parents);
        }
    }
}