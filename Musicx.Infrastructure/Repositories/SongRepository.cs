using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Musicx.Core.Interfaces;
using Musicx.Core.Logging;
using Musicx.Data;
using Musicx.Data.Entities;

namespace Musicx.Infrastructure.Repositories;

public interface ISongRepository : IRepository<SongEntity>;

public class SongRepository(AppDbContext context, ILoggerFactory loggerFactory) : ISongRepository
{
    private readonly ILogger _logger = loggerFactory.CreateLogger(typeof(SongRepository));

    public async Task<SongEntity?> FindById(ulong id)
    {
        return await context.Songs.FindAsync(id);
    }

    public async Task<List<SongEntity>> FindAll(int skip = 0, int count = 100,
        Expression<Func<SongEntity, bool>>? filter = null)
    {
        var query = context.Songs.AsQueryable();

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

    public async Task Save(SongEntity song)
    {
        await SaveAll([song]);
    }

    public async Task SaveAll(IList<SongEntity> songs)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            foreach (var song in songs)
            {
                if (0 == song.Id)
                {
                    context.Songs.Add(song);
                }
                else
                {
                    context.Songs.Update(song);
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
            var song = await context.Songs.FindAsync(id);

            if (null != song)
            {
                context.Songs.Remove(song);
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