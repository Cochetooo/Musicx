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

internal sealed class ArtistRepository(
    ApiDbContext context,
    ILoggerProvider loggerProvider) : IArtistRepository
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(ArtistRepository));
    
    public async Task DeleteAsync(long id)
    {
        _logger.LogDebug($"📄 SQL : DELETE FROM artists WHERE id = {id}");
        
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
            var deleteAllSql = "DELETE FROM \"Artists\" WHERE \"Id\" IN (@ids)";

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

    public async Task<Artist?> FindByIdAsync(long id, IQuerySpecification<Artist>? artistQuerySpecification = null)
    {
        _logger.LogDebug($"📄 SQL : SELECT * FROM artists WHERE id = {id}");
        
        var artistSet = context.Artists;
        GetIncludes(artistSet, artistQuerySpecification);
        
        return await artistSet
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Artist>> FindAsync(int skip = 0, int take = 100, Expression<Func<Artist, bool>>? filter = null,
        IQuerySpecification<Artist>? artistQuerySpecification = null)
    {
        _logger.LogDebug("📄 SQL : SELECT * FROM artists");
        
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
        _logger.LogDebug("📄 SQL : SELECT * FROM artists WHERE id IN ({Ids})", string.Join(",", ids));

        var enumerable = ids as long[] ?? ids.ToArray();
        
        if (0 == enumerable.Length)
        {
            _logger.LogDebug("ℹ️ FIND IN Artist : No entry found.");
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
        _logger.LogDebug($"📄 SQL : INSERT INTO artists (name, artwork_url, country) " +
                         $"VALUES ('{entity.Name}', '{entity.ArtworkUrl}', '{entity.Country}')");
        
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
            await transaction.RollbackAsync();
            throw new RepositoryException("❌ SAVE Artist : Could not persist.", ex, _logger);
        }
    }

    public async Task<List<long>> SaveAllAsync(IEnumerable<Artist> entities)
    {
        _logger.LogDebug("📄 SAVE ALL Artist");
        
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
            await transaction.RollbackAsync();
            throw new RepositoryException("❌ SAVE ALL Artist : Could not persist", ex, _logger);
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