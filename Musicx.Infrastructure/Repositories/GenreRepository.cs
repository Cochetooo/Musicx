using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Musicx.Core.Interfaces;
using Musicx.Core.Logging;
using Musicx.Data;
using Musicx.Data.Entities;

namespace Musicx.Infrastructure.Repositories;

public interface IGenreRepository : IRepository<GenreEntity>;

public class GenreRepository(AppDbContext context, ILoggerFactory loggerFactory) : IGenreRepository
{
    private readonly ILogger<GenreRepository> _logger = loggerFactory.CreateLogger<GenreRepository>();

    public async Task<GenreEntity?> FindById(ulong id)
    {
        return await context.Genres
            .Include(g => g.Parents)
            .Include(g => g.Children)
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<List<GenreEntity>> FindAll(int skip = 0, int count = 100,
        Expression<Func<GenreEntity, bool>>? filter = null)
    {
        var query = context.Genres
            .Include(g => g.Parents)
            .Include(g => g.Children)
            .AsQueryable();

        if (null != filter)
        {
            query = query.Where(filter);
        }
        
        return await query
            .AsNoTracking()
            .Skip(skip)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<GenreEntity>> FindIn(IList<ulong> ids)
    {
        if (ids.Count == 0)
        {
            return [];
        }
        
        return await context.Genres
            .Where(s => ids.Contains(s.Id))
            .Include(s => s.Parents)
            .Include(s => s.Children)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<uint> GetCount()
    {
        return (uint)await context.Genres.CountAsync();
    }

    public async Task<ulong> Save(GenreEntity genre)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            if (0 == genre.Id)
            {
                context.Genres.Add(genre);
            }
            else
            {
                context.Genres.Update(genre);
            }
            
            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return genre.Id;
        }
        catch (Exception ex)
        {
            _logger.Fatal($"❌ Failed saving", ex);
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<ulong>> SaveAll(IList<GenreEntity> genres)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        var ids = new List<ulong>();

        try
        {
            foreach (var genre in genres)
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

    public async Task Delete(ulong id)
    {
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
            _logger.Fatal($"❌ Failed deleting", ex);
            await transaction.RollbackAsync();
            throw;
        }
    }
}