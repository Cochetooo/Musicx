using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Persistence;
using Musicx.Application.Api.Interfaces.Specifications;
using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Domain.Models;
using Musicx.Infrastructure.Shared.Exceptions;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Repositories;

internal sealed class AlbumRepository(
    ApiDbContext context,
    ILoggerProvider loggerProvider) : IAlbumRepository
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(AlbumRepository));
    
    public async Task DeleteAsync(long id)
    {
        _logger.LogDebug($"📄 SQL : DELETE FROM albums WHERE id = {id}");
        
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
            var deleteAllSql = "DELETE FROM \"Albums\" WHERE \"Id\" IN (@ids)";

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

    public async Task<Album?> FindByIdAsync(long id, IQuerySpecification<Album>? albumQuerySpecification = null)
    {
        _logger.LogDebug($"📄 SQL : SELECT * FROM albums WHERE id = {id}");
        
        var albumSet = context.Albums
            .AsQueryable();
        
        albumSet = GetIncludes(albumSet, albumQuerySpecification);
        
        return await albumSet
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .FirstOrDefaultAsync(s => s.Id == id);
    }
    
    public async Task<List<Album>> FindByArtistIdAsync(long artistId, IQuerySpecification<Album>? albumQuerySpecification = null)
    {
        _logger.LogDebug($"📄 SQL : SELECT * FROM albums WHERE artist_id = {artistId}");
        
        var albumSet = context.Albums
            .AsQueryable();
        
        albumSet = GetIncludes(albumSet, albumQuerySpecification);
        
        return await albumSet
            .AsNoTracking()
            .OrderBy(x => x.ReleaseDate)
                .ThenBy(x => x.Name)
            .Where(s => s.Artist != null && s.Artist.Id == artistId)
            .ToListAsync();
    }

    public async Task<List<Album>> FindAsync(int skip = 0, int take = 100, Expression<Func<Album, bool>>? filter = null,
        IQuerySpecification<Album>? albumQuerySpecification = null)
    {
        _logger.LogDebug("📄 SQL : SELECT * FROM albums");
        
        var albumSet = context.Albums
            .AsQueryable();
        
        albumSet = GetIncludes(albumSet, albumQuerySpecification);

        if (null != filter)
        {
            albumSet = albumSet.Where(filter);
        }
        
        return await albumSet
            .AsNoTracking()
            .Skip(skip)
            .Take(take)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<List<Album>> FindIn(IEnumerable<long> ids, IQuerySpecification<Album>? albumQuerySpecification = null)
    {
        _logger.LogDebug("📄 SQL : SELECT * FROM albums WHERE id IN ({Ids})", string.Join(",", ids));

        var enumerable = ids as long[] ?? ids.ToArray();
        
        if (0 == enumerable.Length)
        {
            _logger.LogDebug("ℹ️ FIND IN Album : No entry found.");
            return [];
        }
        
        var albumSet = context.Albums
            .AsQueryable();
        
        albumSet = GetIncludes(albumSet, albumQuerySpecification);
        
        return await albumSet
            .Where(s => enumerable.Contains(s.Id))
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<int> GetCountAsync()
    {
        return await context.Albums.CountAsync();
    }

    public async Task<long> SaveAsync(Album entity)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            if (0 == entity.Id)
            {
                _logger.LogDebug($"📄 SQL : INSERT INTO albums (name, artwork_url, artist_id...) " +
                                 $"VALUES ('{entity.Name}', '{entity.ArtworkUrl}', '{entity.Artist?.Id}')");
                
                entity.CreatedAt = DateTime.Now;
                entity.UpdatedAt = DateTime.Now;

                if (entity.Artist is not null)
                {
                    context.Attach(entity.Artist);
                }

                foreach (var primaryGenre in entity.PrimaryGenres)
                {
                    context.Attach(primaryGenre);
                }
                
                foreach (var influenceGenre in entity.InfluenceGenres)
                {
                    context.Attach(influenceGenre);
                }

                context.Albums.Add(entity);
            }
            else
            {
                _logger.LogDebug($"📄 SQL : UPDATE artists SET Name={entity.Name}, " +
                                 $"ArtworkUrl={entity.ArtworkUrl}, ReleaseDate={entity.ReleaseDate}" +
                                 $"WHERE Id = {entity.Id}");
                
                var existingAlbum = await context.Albums
                    .Include(a => a.PrimaryGenres)
                    .Include(a => a.InfluenceGenres)
                    .FirstAsync(s => s.Id == entity.Id);

                existingAlbum.Name = entity.Name;
                existingAlbum.ArtworkUrl = entity.ArtworkUrl;
                existingAlbum.ReleaseDate = entity.ReleaseDate;
                existingAlbum.IsFarRight = entity.IsFarRight;
                existingAlbum.DiscTotal = entity.DiscTotal;
                existingAlbum.TrackTotal = entity.TrackTotal;
                existingAlbum.UpdatedAt = DateTime.Now;
                
                existingAlbum.PrimaryGenres.Clear();
                foreach (var g in entity.PrimaryGenres)
                {
                    existingAlbum.PrimaryGenres.Add(context.Attach(g).Entity);
                }
                
                existingAlbum.InfluenceGenres.Clear();
                foreach (var g in entity.InfluenceGenres)
                {
                    existingAlbum.InfluenceGenres.Add(context.Attach(g).Entity);
                }
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

    public async Task<List<long>> SaveAllAsync(IEnumerable<Album> entities)
    {
        _logger.LogDebug("📄 SAVE ALL Album");
        
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
            await transaction.RollbackAsync();
            throw new RepositoryException("❌ SAVE ALL Artist : Could not persist", ex, _logger);
        }
    }

    private static IQueryable<Album> GetIncludes(IQueryable<Album> query, IQuerySpecification<Album>? querySpecification = null)
    {
        if (querySpecification is not AlbumQuerySpecification albumQuerySpecification)
            return query;

        if (albumQuerySpecification.IncludeArtist)
        {
            query = query.Include(s => s.Artist);
        }

        if (albumQuerySpecification.IncludePrimaryGenres)
        {
            query = query.Include(s => s.PrimaryGenres);
        }

        if (albumQuerySpecification.IncludeInfluenceGenres)
        {
            query = query.Include(s => s.InfluenceGenres);
        }

        return query;
    }
}