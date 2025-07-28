using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Musicx.Application.Desktop.Interfaces.Persistence;
using Musicx.Application.Desktop.Specifications;
using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Application.Shared.Interfaces.Persistence;


namespace Musicx.Infrastructure.Desktop.Persistence.Repositories;

internal sealed class GenreRepository(
    AppDbContext context,
    IGenreCache genreCache,
    ILoggerProvider loggerProvider) : IGenreRepository
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(GenreRepository));
    
    public async Task DeleteAsync(long id)
    {
        _logger.LogDebug($"📄 Delete Genre : {id}");
        
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
            _logger.LogCritical($"❌ Could not delete id {id}", ex);
            await transaction.RollbackAsync();
            throw;
        }
    }

    public Task DeleteAllAsync(IEnumerable<long> ids)
    {
        throw new NotImplementedException();
    }

    public async Task<Genre?> FindByIdAsync(long id, IQuerySpecification<Genre>? genreQuerySpecification = null)
    {
        _logger.LogDebug($"📄 Find By Id Genre : {id}");
        
        var genreSet = context.Genres;
        GetIncludes(genreSet, genreQuerySpecification);
        
        return await genreSet
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Genre>> FindAsync(int skip = 0, int take = 100, Expression<Func<Genre, bool>>? filter = null,
        IQuerySpecification<Genre>? genreQuerySpecification = null)
    {
        _logger.LogDebug($"📄 Find Genre");
        
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
        _logger.LogDebug("📄 Find In Genre");

        var enumerable = ids as long[] ?? ids.ToArray();
        
        if (0 == enumerable.Length)
        {
            _logger.LogDebug("ℹ️ Empty List in Find In");
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
        _logger.LogDebug("📄 Save Genre");
        
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
            _logger.LogCritical("❌ Failed saving.", ex);
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<long>> SaveAllAsync(IEnumerable<Genre> entities)
    {
        _logger.LogDebug("📄 SaveAll Genre");
        
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
            _logger.LogCritical($"❌ Failed saving", ex);
            await transaction.RollbackAsync();
            throw;
        }
    }

    private static void GetIncludes(in DbSet<Genre> genreSet, IQuerySpecification<Genre>? querySpecification = null)
    {
        if (null == querySpecification)
        {
            return;
        }
        
        var genreQuerySpecification = (GenreQuerySpecification)querySpecification;

        /*if (genreQuerySpecification.IncludeChildren)
        {
            genreSet.Include(g => g.Children);
        }

        if (genreQuerySpecification.IncludeParents)
        {
            genreSet.Include(g => g.Parents);
        }*/
    }
}