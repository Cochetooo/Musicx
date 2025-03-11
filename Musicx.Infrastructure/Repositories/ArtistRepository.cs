using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Musicx.Core.Interfaces;
using Musicx.Core.Logging;
using Musicx.Core.Models;
using Musicx.Data;
using Musicx.Data.Entities;

namespace Musicx.Infrastructure.Repositories;

public interface IArtistRepository : IRepository<ArtistEntity>;

/// <summary>
/// Manipulate artist data with EntityFramework.
/// </summary>
/// <author>Cochetooo</author>
/// <since>0.6.0</since>
public class ArtistRepository(AppDbContext context, ILoggerFactory loggerFactory) : IArtistRepository
{
    private readonly ILogger<ArtistRepository> _logger = loggerFactory.CreateLogger<ArtistRepository>();

    public async Task<ArtistEntity?> FindById(ulong id)
    {
        return await context.Artists.FindAsync(id);
    }

    public async Task<List<ArtistEntity>> FindAll(int skip = 0, int count = 100,
        Expression<Func<ArtistEntity, bool>>? filter = null)
    {
        var query = context.Artists.AsQueryable();

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
    
    public async Task<uint> GetCount()
    {
        return (uint)await context.Artists.CountAsync();
    }

    public async Task Save(ArtistEntity artist)
    {
        await SaveAll([artist]);
    }

    public async Task SaveAll(IList<ArtistEntity> artists)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            foreach (var artist in artists)
            {
                if (0 == artist.Id)
                {
                    context.Artists.Add(artist);
                }
                else
                {
                    context.Artists.Update(artist);
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
            var artist = await context.Artists.FindAsync(id);

            if (null != artist)
            {
                context.Artists.Remove(artist);
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