using System.Linq.Expressions;
using System.Net.Mime;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Persistence;
using Musicx.Application.Api.Interfaces.Specifications;
using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Infrastructure.Desktop.Persistence;
using Musicx.Infrastructure.Shared.Exceptions;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Repositories;

internal sealed class ArtistRepository(
    ILoggerProvider loggerProvider) : IArtistRepository
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(ArtistRepository));
    
    public async Task DeleteAsync(long id)
    {
        _logger.LogDebug($"📄 SQL : DELETE FROM artists WHERE id = {id}");
        
        /*await using var transaction = await context.Database.BeginTransactionAsync();

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
        }*/
    }

    public async Task DeleteAllAsync(IEnumerable<long> ids)
    {
        var stringIds = string.Join(",", ids);

        /*await using var transaction = await context.Database.BeginTransactionAsync();
        
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
        }*/
    }

    public async Task<OutArtist?> FindByIdAsync(long id, IQuerySpecification<InArtist>? artistQuerySpecification = null)
    {
        _logger.LogDebug($"📄 SQL : SELECT * FROM artists WHERE id = {id}");
        return null;

        /*var artistSet = context.Artists;

        return await artistSet
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .FirstOrDefaultAsync(s => s.Id == id);*/
    }

    public async Task<List<OutArtist>> FindAsync(int skip = 0, int take = 100, Expression<Func<InArtist, bool>>? filter = null,
        IQuerySpecification<InArtist>? artistQuerySpecification = null)
    {
        _logger.LogDebug("📄 SQL : SELECT * FROM artists");
        return [];

        /*var artistSet = context.Artists;

        var query = artistSet.AsQueryable();

        if (null != filter)
        {
            //query = query.Where(filter);
        }

        return await query
            .AsNoTracking()
            .Skip(skip)
            .Take(take)
            .OrderBy(x => x.Name)
            .ToListAsync();*/
    }

    public async Task<List<OutArtist>> FindIn(IEnumerable<long> ids, IQuerySpecification<InArtist>? artistQuerySpecification = null)
    {
        _logger.LogDebug("📄 SQL : SELECT * FROM artists WHERE id IN ({Ids})", string.Join(",", ids));

        return [];
        /*var enumerable = ids as long[] ?? ids.ToArray();

        if (0 == enumerable.Length)
        {
            _logger.LogDebug("ℹ️ FIND IN Artist : No entry found.");
            return [];
        }

        var artistSet = context.Artists;

        return await artistSet
            .Where(s => enumerable.Contains(s.Id))
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync();*/
    }

    public async Task<int> GetCountAsync()
    {
        return 1;
    }

    public async Task<long> SaveAsync(InArtist entity)
    {
        /*await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            if (0 == entity.Id)
            {
                _logger.LogDebug($"📄 SQL : INSERT INTO artists (name, artwork_url, country) " +
                                 $"VALUES ('{entity.Name}', '{entity.ArtworkUrl}', '{entity.Country}')");
                
                //entity.CreatedAt = DateTime.Now;
                //entity.UpdatedAt = DateTime.Now;
                
                //context.Artists.Add(entity);
            }
            else
            {
                _logger.LogDebug($"📄 SQL : UPDATE artists SET Name={entity.Name}, " +
                                 $"ArtworkUrl={entity.ArtworkUrl}, Country={entity.Country}" +
                                 $"WHERE Id = {entity.Id}");
                
                //entity.UpdatedAt = DateTime.Now;
                context.Artists.Update(entity);
                context.Entry(entity).Property(x => x.CreatedAt).IsModified = false;
            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return entity.Id;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new RepositoryException("❌ SAVE Artist : Could not persist.", ex, _logger);
        }*/
        throw new NotImplementedException();
    }

    public async Task<List<long>> SaveAllAsync(IEnumerable<InArtist> entities)
    {
        /*_logger.LogDebug("📄 SAVE ALL Artist");
        
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
        }*/
        
        throw new NotImplementedException();
    }
}