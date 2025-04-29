using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Application.Desktop.Interfaces.Persistence;
using Musicx.Application.Desktop.Specifications;
using Musicx.Domain.Models;

namespace Musicx.Infrastructure.Persistence.Repositories;

internal sealed class ReleaseRepository(
    AppDbContext context,
    IReleaseCache releaseCache,
    ILoggerFactory loggerFactory) : IReleaseRepository
{
    private readonly ILogger<ReleaseRepository> _logger = loggerFactory.CreateLogger<ReleaseRepository>();
    
    public async Task DeleteAsync(long id)
    {
        _logger.Db($"📄 Delete Release : {id}");
        
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var release = await context.Releases.FindAsync(id);

            if (null != release)
            {
                context.Releases.Remove(release);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.Fatal($"❌ Could not delete id {id}", ex);
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Release?> FindByIdAsync(long id, IQuerySpecification<Release>? releaseQuerySpecification = null)
    {
        _logger.Db($"📄 Find By Id Release : {id}");
        
        var releaseSet = context.Releases;
        GetIncludes(releaseSet, releaseQuerySpecification);
        
        return await releaseSet
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Release>> FindAsync(int skip = 0, int take = 100, Expression<Func<Release, bool>>? filter = null,
        IQuerySpecification<Release>? releaseQuerySpecification = null)
    {
        _logger.Db($"📄 Find Release");
        
        var releaseSet = context.Releases;
        
        GetIncludes(releaseSet, releaseQuerySpecification);
        var query = releaseSet.AsQueryable();

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

    public async Task<List<Release>> FindIn(IEnumerable<long> ids, IQuerySpecification<Release>? releaseQuerySpecification = null)
    {
        _logger.Db("📄 Find In Release");

        var enumerable = ids as long[] ?? ids.ToArray();
        
        if (0 == enumerable.Length)
        {
            _logger.Debug("ℹ️ Empty List in Find In");
            return [];
        }
        
        var releaseSet = context.Releases;
        GetIncludes(releaseSet, releaseQuerySpecification);
        
        return await releaseSet
            .Where(s => enumerable.Contains(s.Id))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<int> GetCountAsync()
    {
        return await context.Releases.CountAsync();
    }

    public async Task<long> SaveAsync(Release entity)
    {
        _logger.Db("📄 Save Release");
        
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            if (0 == entity.Id)
            {
                context.Releases.Add(entity);
            }
            else
            {
                context.Releases.Update(entity);
            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return entity.Id;
        }
        catch (Exception ex)
        {
            _logger.Fatal("❌ Failed saving.", ex);
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<long>> SaveAllAsync(IEnumerable<Release> entities)
    {
        _logger.Db("📄 SaveAll Release");
        
        await using var transaction = await context.Database.BeginTransactionAsync();

        var ids = new List<long>();

        try
        {
            foreach (var release in entities)
            {
                if (0 == release.Id)
                {
                    context.Releases.Add(release);
                }
                else
                {
                    context.Releases.Update(release);
                }
                
                ids.Add(release.Id);
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

    private void GetIncludes(in DbSet<Release> releaseSet, IQuerySpecification<Release>? querySpecification = null)
    {
        if (null == querySpecification)
        {
            return;
        }
        
        var releaseQuerySpecification = (ReleaseQuerySpecification)querySpecification;

        if (releaseQuerySpecification.IncludeAlbum)
        {
            releaseSet.Include(s => s.Album);
        }

        if (releaseQuerySpecification.IncludeLabel)
        {
            releaseSet.Include(s => s.Label);
        }
    }
}