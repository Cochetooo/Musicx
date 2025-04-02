using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Musicx.Application.Common.Interfaces.Common;
using Musicx.Application.Common.Interfaces.Persistence;
using Musicx.Application.Desktop.Interfaces.Persistence;
using Musicx.Application.Desktop.Specifications;
using Musicx.Domain.Entities;

namespace Musicx.Infrastructure.Desktop.Persistence.Repositories;

public class SongRepository(
    AppDbContext context,
    ISongCache songCache,
    ILoggerFactory loggerFactory) : ISongRepository
{
    private readonly ILogger<SongRepository> _logger = loggerFactory.CreateLogger<SongRepository>();
    
    public async Task DeleteAsync(long id)
    {
        _logger.Db($"📄 Delete Song : {id}");
        
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
            _logger.Fatal($"❌ Could not delete id {id}", ex);
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Song?> FindByIdAsync(long id, IQuerySpecification<Song>? songQuerySpecification = null)
    {
        _logger.Db($"📄 Find By Id Song : {id}");
        
        var songSet = context.Songs;
        GetIncludes(songSet, songQuerySpecification);
        
        return await songSet
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Song>> FindAsync(int skip = 0, int take = 100, Expression<Func<Song, bool>>? filter = null,
        IQuerySpecification<Song>? songQuerySpecification = null)
    {
        _logger.Db($"📄 Find Song");
        
        var songSet = context.Songs;
        
        GetIncludes(songSet, songQuerySpecification);
        var query = songSet.AsQueryable();

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

    public async Task<List<Song>> FindIn(IEnumerable<long> ids, IQuerySpecification<Song>? songQuerySpecification = null)
    {
        _logger.Db("📄 Find In Song");

        var enumerable = ids as long[] ?? ids.ToArray();
        
        if (!enumerable.Any())
        {
            _logger.Debug("ℹ️ Empty List in Find In");
            return [];
        }
        
        var songSet = context.Songs;
        GetIncludes(songSet, songQuerySpecification);
        
        return await songSet
            .Where(s => enumerable.Contains(s.Id))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<int> GetCountAsync()
    {
        return await context.Songs.CountAsync();
    }

    public async Task<long> SaveAsync(Song entity)
    {
        _logger.Db("📄 Save Song");
        
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            if (0 == entity.Id)
            {
                context.Songs.Add(entity);
            }
            else
            {
                context.Songs.Update(entity);
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

    public async Task<List<long>> SaveAllAsync(IEnumerable<Song> entities)
    {
        _logger.Db("📄 SaveAll Song");
        
        await using var transaction = await context.Database.BeginTransactionAsync();

        var ids = new List<long>();

        try
        {
            foreach (var song in entities)
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

    private void GetIncludes(in DbSet<Song> songSet, IQuerySpecification<Song>? querySpecification = null)
    {
        if (null == querySpecification)
        {
            return;
        }
        
        var songQuerySpecification = (SongQuerySpecification)querySpecification;

        if (songQuerySpecification.IncludeArtist)
        {
            songSet.Include(s => s.Artist);
        }

        if (songQuerySpecification.IncludeAlbum)
        {
            if (songQuerySpecification.IncludeAlbumArtist)
            {
                songSet.Include(s => s.Album)
                    .ThenInclude(a => a.Artist);
            }
            else
            {
                songSet.Include(s => s.Album);
            }
        }

        if (songQuerySpecification.IncludePrimaryGenres)
        {
            songSet.Include(s => s.PrimaryGenres);
        }

        if (songQuerySpecification.IncludeInfluenceGenres)
        {
            songSet.Include(s => s.InfluenceGenres);
        }
    }
}