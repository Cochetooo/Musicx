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
    private readonly ILogger _logger = loggerFactory.CreateLogger(typeof(GenreRepository));

    public async Task<GenreEntity?> FindById(ulong id)
    {
        return await context.Genres.FindAsync(id);
    }

    public async Task<List<GenreEntity>> FindAll(int skip = 0, int count = 100,
        Expression<Func<GenreEntity, bool>>? filter = null)
    {
        var query = context.Genres.AsQueryable();

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

    public async Task Save(GenreEntity genre)
    {
        await SaveAll([genre]);
    }

    public async Task SaveAll(IList<GenreEntity> genres)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

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
            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
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