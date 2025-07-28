using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Musicx.Application.Desktop.Interfaces.Persistence;
using Musicx.Application.Desktop.Specifications;
using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Application.Shared.Interfaces.Persistence;


namespace Musicx.Infrastructure.Desktop.Persistence.Repositories;

internal sealed class ArtistRepository(
    AppDbContext context,
    IArtistCache artistCache,
    ILoggerProvider loggerProvider) : IArtistRepository
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(ArtistRepository));
    
    public async Task DeleteAsync(long id)
    {
        _logger.LogDebug($"📄 Delete Artist : {id}");
        
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
            _logger.LogCritical($"❌ Could not delete id {id}", ex);
            await transaction.RollbackAsync();
            throw;
        }
    }

    public Task DeleteAllAsync(IEnumerable<long> ids)
    {
        throw new NotImplementedException();
    }

    public async Task<Artist?> FindByIdAsync(long id, IQuerySpecification<Artist>? artistQuerySpecification = null)
    {
        _logger.LogDebug($"📄 Find By Id Artist : {id}");
        
        var artistSet = context.Artists;
        GetIncludes(artistSet, artistQuerySpecification);
        
        return await artistSet
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Artist>> FindAsync(int skip = 0, int take = 100, Expression<Func<Artist, bool>>? filter = null,
        IQuerySpecification<Artist>? artistQuerySpecification = null)
    {
        _logger.LogDebug($"📄 Find Artist");
        
        var artistSet = context.Artists;
        
        GetIncludes(artistSet, artistQuerySpecification);
        var query = artistSet.AsQueryable();

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

    public async Task<List<Artist>> FindIn(IEnumerable<long> ids, IQuerySpecification<Artist>? artistQuerySpecification = null)
    {
        _logger.LogDebug("📄 Find In Artist");

        var enumerable = ids as long[] ?? ids.ToArray();
        
        if (0 == enumerable.Length)
        {
            _logger.LogDebug("ℹ️ Empty List in Find In");
            return [];
        }
        
        var artistSet = context.Artists;
        GetIncludes(artistSet, artistQuerySpecification);
        
        return await artistSet
            .Where(s => enumerable.Contains(s.Id))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<int> GetCountAsync()
    {
        return await context.Artists.CountAsync();
    }

    public async Task<long> SaveAsync(Artist entity)
    {
        _logger.LogDebug("📄 Save Artist");
        
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            if (0 == entity.Id)
            {
                context.Artists.Add(entity);
            }
            else
            {
                context.Artists.Update(entity);
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

    public async Task<List<long>> SaveAllAsync(IEnumerable<Artist> entities)
    {
        _logger.LogDebug("📄 SaveAll Artist");
        
        await using var transaction = await context.Database.BeginTransactionAsync();

        var ids = new List<long>();

        try
        {
            foreach (var artist in entities)
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
            _logger.LogCritical($"❌ Failed saving", ex);
            await transaction.RollbackAsync();
            throw;
        }
    }

    private static void GetIncludes(in DbSet<Artist> artistSet, IQuerySpecification<Artist>? querySpecification = null)
    {
        if (null == querySpecification)
        {
            return;
        }
        
        var artistQuerySpecification = (ArtistQuerySpecification)querySpecification;

        
    }
}