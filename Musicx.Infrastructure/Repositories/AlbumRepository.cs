using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Musicx.Core.Interfaces;
using Musicx.Core.Logging;
using Musicx.Core.Models;
using Musicx.Data;
using Musicx.Data.Entities;

namespace Musicx.Infrastructure.Repositories;

public interface IAlbumRepository : IRepository<AlbumEntity>;

public class AlbumRepository(AppDbContext context, ILoggerFactory loggerFactory) : IAlbumRepository
{
    private readonly ILogger<AlbumRepository> _logger = loggerFactory.CreateLogger<AlbumRepository>();

    public async Task<AlbumEntity?> FindById(ulong id)
    {
        return await context.Albums
            .Include(a => a.Genres)
            .Include(a => a.InfluenceGenres)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<List<AlbumEntity>> FindAll(int skip = 0, int count = 100,
        Expression<Func<AlbumEntity, bool>>? filter = null)
    {
        var query = context.Albums
            .Include(a => a.Genres)
            .Include(a => a.InfluenceGenres)
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
    
    public async Task<List<AlbumEntity>> FindIn(IList<ulong> ids)
    {
        if (ids.Count == 0)
        {
            return [];
        }
        
        return await context.Albums
            .Where(s => ids.Contains(s.Id))
            .Include(s => s.Genres)
            .Include(s => s.InfluenceGenres)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<uint> GetCount()
    {
        return (uint)await context.Albums.CountAsync();
    }

    public async Task<ulong> Save(AlbumEntity album)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            if (0 == album.Id)
            {
                context.Albums.Add(album);
            }
            else
            {
                context.Albums.Update(album);
            }
            
            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return album.Id;
        }
        catch (Exception ex)
        {
            _logger.Fatal($"❌ Failed saving", ex);
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<ulong>> SaveAll(IList<AlbumEntity> albums)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        var ids = new List<ulong>();

        try
        {
            foreach (var album in albums)
            {
                if (0 == album.Id)
                {
                    context.Albums.Add(album);
                }
                else
                {
                    context.Albums.Update(album);
                }
                
                ids.Add(album.Id);
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
            var album = await context.Albums.FindAsync(id);

            if (null != album)
            {
                context.Albums.Remove(album);
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