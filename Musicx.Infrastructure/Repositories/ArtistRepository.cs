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
        var artist = await context.Artists.FirstOrDefaultAsync(a => a.Id == id);

        if (artist is BandArtistEntity)
        {
            return await context.Artists
                .OfType<BandArtistEntity>()
                .Include(b => b.Members)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);
        }
        
        if (artist is PersonArtistEntity)
        {
            return await context.Artists
                .OfType<PersonArtistEntity>()
                .Include(p => p.Bands)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        return artist;
    }

    public async Task<List<ArtistEntity>> FindAll(int skip = 0, int count = 100,
        Expression<Func<ArtistEntity, bool>>? filter = null)
    {
        var query = context.Artists.AsQueryable();

        if (null != filter)
        {
            query = query.Where(filter);
        }

        var bandQuery = query
            .OfType<BandArtistEntity>()
            .Include(b => b.Members);
        
        var personQuery = query
            .OfType<PersonArtistEntity>()
            .Include(p => p.Bands);
        
        // Récupérer les résultats séparément
        var bands = await bandQuery
            .AsNoTracking()
            .Skip(skip)
            .Take(count)
            .ToListAsync();

        var persons = await personQuery
            .AsNoTracking()
            .Skip(skip)
            .Take(count)
            .ToListAsync();

        // Combiner les deux listes en mémoire
        var combined = bands.Concat(persons.Cast<ArtistEntity>()).ToList();

        return combined;
    }

    public async Task<List<ArtistEntity>> FindIn(IList<ulong> ids)
    {
        if (ids.Count == 0)
        {
            return [];
        }
        
        var query = context.Artists.AsQueryable()
            .Where(a => ids.Contains(a.Id));
        
        var bandQuery = query
            .OfType<BandArtistEntity>()
            .Include(b => b.Members);
        
        var personQuery = query
            .OfType<PersonArtistEntity>()
            .Include(p => p.Bands);
        
        return await bandQuery
            .Concat<ArtistEntity>(personQuery)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<uint> GetCount()
    {
        return (uint)await context.Artists.CountAsync();
    }

    public async Task<ulong> Save(ArtistEntity artist)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            if (0 == artist.Id)
            {
                context.Artists.Add(artist);
            }
            else
            {
                context.Artists.Update(artist);
            }
            
            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return artist.Id;
        }
        catch (Exception ex)
        {
            _logger.Fatal($"❌ Failed saving", ex);
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<ulong>> SaveAll(IList<ArtistEntity> artists)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        var ids = new List<ulong>();

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
                
                ids.Add(artist.Id);
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