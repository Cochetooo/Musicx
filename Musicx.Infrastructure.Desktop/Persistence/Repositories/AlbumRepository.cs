using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Musicx.Application.Common.Interfaces.Common;
using Musicx.Application.Common.Interfaces.Persistence;
using Musicx.Application.Desktop.Interfaces.Persistence;
using Musicx.Application.Desktop.Specifications;
using Musicx.Domain.Entities;

namespace Musicx.Infrastructure.Desktop.Persistence.Repositories;

public class AlbumRepository(
    AppDbContext context,
    IAlbumCache albumCache,
    ILoggerFactory loggerFactory) : IAlbumRepository
{
    private readonly ILogger<AlbumRepository> _logger = loggerFactory.CreateLogger<AlbumRepository>();
    
    public async Task DeleteAsync(long id)
    {
        _logger.Db($"📄 Delete Album : {id}");
        
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
            _logger.Fatal($"❌ Could not delete id {id}", ex);
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Album?> FindByIdAsync(long id, IQuerySpecification<Album>? albumQuerySpecification = null)
    {
        _logger.Db($"📄 Find By Id Album : {id}");
        
        var albumSet = context.Albums;
        GetIncludes(albumSet, albumQuerySpecification);
        
        return await albumSet
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Album>> FindAsync(int skip = 0, int take = 100, Expression<Func<Album, bool>>? filter = null,
        IQuerySpecification<Album>? albumQuerySpecification = null)
    {
        _logger.Db($"📄 Find Album");
        
        var albumSet = context.Albums;
        
        GetIncludes(albumSet, albumQuerySpecification);
        var query = albumSet.AsQueryable();

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

    public async Task<List<Album>> FindIn(IEnumerable<long> ids, IQuerySpecification<Album>? albumQuerySpecification = null)
    {
        _logger.Db("📄 Find In Album");

        var enumerable = ids as long[] ?? ids.ToArray();
        
        if (!enumerable.Any())
        {
            _logger.Debug("ℹ️ Empty List in Find In");
            return [];
        }
        
        var albumSet = context.Albums;
        GetIncludes(albumSet, albumQuerySpecification);
        
        return await albumSet
            .Where(s => enumerable.Contains(s.Id))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<int> GetCountAsync()
    {
        return await context.Albums.CountAsync();
    }

    public async Task<long> SaveAsync(Album entity)
    {
        _logger.Db("📄 Save Album");
        
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            if (0 == entity.Id)
            {
                context.Albums.Add(entity);
            }
            else
            {
                context.Albums.Update(entity);
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

    public async Task<List<long>> SaveAllAsync(IEnumerable<Album> entities)
    {
        _logger.Db("📄 SaveAll Album");
        
        await using var transaction = await context.Database.BeginTransactionAsync();

        var ids = new List<long>();

        try
        {
            foreach (var album in entities)
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

    private void GetIncludes(in DbSet<Album> albumSet, IQuerySpecification<Album>? querySpecification = null)
    {
        if (null == querySpecification)
        {
            return;
        }
        
        var albumQuerySpecification = (AlbumQuerySpecification)querySpecification;

        if (albumQuerySpecification.IncludeArtist)
        {
            albumSet.Include(s => s.Artist);
        }

        if (albumQuerySpecification.IncludePrimaryGenres)
        {
            albumSet.Include(s => s.PrimaryGenres);
        }

        if (albumQuerySpecification.IncludeInfluenceGenres)
        {
            albumSet.Include(s => s.InfluenceGenres);
        }
    }
}