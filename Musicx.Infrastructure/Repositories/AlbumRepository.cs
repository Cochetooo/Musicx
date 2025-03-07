using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Musicx.Core.Interfaces;
using Musicx.Core.Logging;
using Musicx.Data;
using Musicx.Data.Entities;

namespace Musicx.Infrastructure.Repositories;

public class AlbumRepository(AppDbContext context, ILoggerFactory loggerFactory) : IRepository<AlbumEntity>
{
    private readonly ILogger _logger = loggerFactory.CreateLogger(typeof(AlbumRepository));

    public async Task<AlbumEntity?> FindById(ulong id)
    {
        return await context.Albums.FindAsync(id);
    }

    public async Task<IEnumerable<AlbumEntity>> FindAll(int skip = 0, int count = 100,
        Expression<Func<AlbumEntity, bool>>? filter = null)
    {
        var query = context.Albums.AsQueryable();

        if (null != filter)
        {
            query = query.Where(filter);
        }
        
        return await query
            .Skip(skip)
            .Take(count)
            .ToListAsync();
    }

    public async Task Save(AlbumEntity album)
    {
        await SaveAll([album]);
    }

    public async Task SaveAll(IList<AlbumEntity> albums)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

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