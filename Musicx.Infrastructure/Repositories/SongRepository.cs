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
    private readonly ILogger<SongRepository> _logger = loggerFactory.CreateLogger<SongRepository>();

    public async Task<SongEntity?> FindById(ulong id)
    {
        return await context.Songs
            .Include(s => s.Genres)
            .Include(s => s.InfluenceGenres)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<SongEntity>> FindAll(int skip = 0, int count = 100,
        Expression<Func<SongEntity, bool>>? filter = null)
    {
        var query = context.Songs
            .Include(s => s.Genres)
            .Include(s => s.InfluenceGenres)
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

    public async Task<List<SongEntity>> FindIn(IList<ulong> ids)
    {
        if (ids.Count == 0)
        {
            return [];
        }
        
        return await context.Songs
            .Where(s => ids.Contains(s.Id))
            .Include(s => s.Genres)
            .Include(s => s.InfluenceGenres)
            .AsNoTracking()
            .ToListAsync();
    }
    
    public async Task<uint> GetCount()
    {
        return (uint)await context.Songs.CountAsync();
    }

    public async Task<ulong> Save(SongEntity song)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            if (0 == song.Id)
            {
                context.Songs.Add(song);
            }
            else
            {
                context.Songs.Update(song);
            }
            
            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return song.Id;
        }
        catch (Exception ex)
        {
            _logger.Fatal($"❌ Failed saving", ex);
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<ulong>> SaveAll(IList<SongEntity> songs)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        var ids = new List<ulong>();

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
                
                ids.Add(song.Id);
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