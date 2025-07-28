using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Specifications;
using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Application.Shared.Interfaces.Persistence;

using Musicx.Infrastructure.Shared.Exceptions;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;
using ISongRepository = Musicx.Application.Api.Interfaces.Persistence.ISongRepository;

namespace Musicx.Infrastructure.API.Persistence.Repositories;

internal sealed class SongRepository(
    ApiDbContext context,
    ILoggerProvider loggerProvider) : ISongRepository
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(SongRepository));
    
    public async Task DeleteAsync(long id)
    {
        _logger.LogDebug($"📄 SQL : DELETE FROM songs WHERE id = {id}");
        
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
            await transaction.RollbackAsync();
            throw new RepositoryException($"❌ Could not delete id {id}", ex, _logger);
        }
    }

    public async Task DeleteAllAsync(IEnumerable<long> ids)
    {
        var stringIds = string.Join(",", ids);

        await using var transaction = await context.Database.BeginTransactionAsync();
        
        try
        {
            var deleteAllSql = "DELETE FROM \"Songs\" WHERE \"Id\" IN (@ids)";

            var parameters = new List<NpgsqlParameter>
            {
                new("@Ids", stringIds)
            };

            _logger.LogDebug(SqlDebugHelper.InterpolateQuery(deleteAllSql, parameters));
            
            await context.Database.ExecuteSqlRawAsync(deleteAllSql, parameters.Cast<object>().ToArray());
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new RepositoryException($"📜❌ Could not delete ids {stringIds}", ex, _logger);
        }
    }

    public async Task<Song?> FindByIdAsync(long id, IQuerySpecification<Song>? songQuerySpecification = null)
    {
        _logger.LogDebug($"📄 SQL : SELECT * FROM songs WHERE id = {id}");
        
        var songSet = context.Songs
            .AsQueryable();
        
        songSet = GetIncludes(songSet, songQuerySpecification);
        
        return await songSet
            .AsNoTracking()
            .OrderBy(x => x.DiscNumber)
            .ThenBy(x => x.TrackNumber)
            .ThenBy(x => x.Title)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Song>> FindAsync(int skip = 0, int take = 100, Expression<Func<Song, bool>>? filter = null,
        IQuerySpecification<Song>? songQuerySpecification = null)
    {
        _logger.LogDebug($"📄 SQL : SELECT * FROM songs");
        
        var songSet = context.Songs
            .AsQueryable();
        
        songSet = GetIncludes(songSet, songQuerySpecification);

        if (null != filter)
        {
            songSet = songSet.Where(filter);
        }
        
        return await songSet
            .AsNoTracking()
            .Skip(skip)
            .Take(take)
            .OrderBy(x => x.DiscNumber)
            .ThenBy(x => x.TrackNumber)
            .ThenBy(x => x.Title)
            .ToListAsync();
    }

    public async Task<List<Song>> FindIn(IEnumerable<long> ids, IQuerySpecification<Song>? songQuerySpecification = null)
    {
        _logger.LogDebug("📄 SQL : SELECT * FROM songs WHERE id IN ({Ids})", string.Join(",", ids));

        var enumerable = ids as long[] ?? ids.ToArray();
        
        if (0 == enumerable.Length)
        {
            _logger.LogDebug("ℹ️ FIND IN Song : No entry found.");
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
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            if (0 == entity.Id)
            {
                _logger.LogDebug($"📄 SQL : INSERT INTO songs (title, album_id, artist_id...) " +
                                 $"VALUES ('{entity.Title}', '{entity.AlbumId}', '{entity.ArtistId}')");

                entity.CreatedAt = DateTime.Now;
                entity.UpdatedAt = DateTime.Now;
                
                context.Songs.Add(entity);
            }
            else
            {
                _logger.LogDebug($"📄 SQL : UPDATE artists SET Title={entity.Title}" +
                                 $"WHERE Id = {entity.Id}");
                
                entity.UpdatedAt = DateTime.Now;
                
                context.Songs.Update(entity);
                context.Entry(entity).Property(x => x.CreatedAt).IsModified = false;
            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return entity.Id;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new RepositoryException("❌ SAVE Album : Could not persist.", ex, _logger);
        }
    }

    public async Task<List<long>> SaveAllAsync(IEnumerable<Song> entities)
    {
        _logger.LogDebug("📄 SAVE ALL Song");
        
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
            await transaction.RollbackAsync();
            throw new RepositoryException("❌ SAVE ALL Artist : Could not persist", ex, _logger);
        }
    }

    private static IQueryable<Song> GetIncludes(IQueryable<Song> query, IQuerySpecification<Song>? querySpecification = null)
    {
        if (querySpecification is not SongQuerySpecification songQuerySpecification)
            return query;

        if (songQuerySpecification.IncludeArtist)
        {
            query = query.Include(s => s.Artist);
        }

        if (songQuerySpecification.IncludeAlbum)
        {
            if (songQuerySpecification.IncludeAlbumArtist)
            {
                query = query.Include(s => s.Album)
                    .ThenInclude(a => a.Artist);
            }
            else
            {
                query = query.Include(s => s.Album);
            }
        }

        if (songQuerySpecification.IncludePrimaryGenres)
        {
            query = query.Include(s => s.PrimaryGenres);
        }

        if (songQuerySpecification.IncludeInfluenceGenres)
        {
            query = query.Include(s => s.InfluenceGenres);
        }

        return query;
    }
}