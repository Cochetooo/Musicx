using System.Linq.Expressions;
using System.Net.Mime;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.Persistence;
using Musicx.Application.Api.Interfaces.Specifications;
using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Application.Shared.Options;
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
        const string sql = "DELETE FROM \"Albums\" WHERE \"Id\" = @id";
        var parameters = new List<NpgsqlParameter>
        {
            new("@id", id)
        };
        
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            _logger.LogDebug(SqlDebugHelper.InterpolateQuery(sql, parameters));
            await context.Database.ExecuteSqlRawAsync(sql, parameters.Cast<object>().ToArray());
            await transaction.CommitAsync();
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
        const string deleteAllSql = "DELETE FROM \"Albums\" WHERE \"Id\" IN (@ids)";
        var parameters = new List<NpgsqlParameter>
        {
            new("@Ids", stringIds)
        };

        await using var transaction = await context.Database.BeginTransactionAsync();
        
        try
        {
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
        var findByIdSql = "SELECT * FROM \"Albums\"";
        var parameters = new List<NpgsqlParameter>
        {
            new("@id", id)
        };

        findByIdSql = AppendJoins(findByIdSql, albumQuerySpecification);
        findByIdSql += " WHERE \"Albums\".\"Id\" = @id";
        
        _logger.LogDebug(SqlDebugHelper.InterpolateQuery(findByIdSql, parameters));
        return await context.Albums
            .FromSqlRaw(findByIdSql, parameters.Cast<object>().ToArray())
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }
    
    public async Task<List<Album>> FindByArtistIdAsync(long artistId, IQuerySpecification<Album>? albumQuerySpecification = null)
    {
        var findByArtistSql = "SELECT * FROM \"Albums\"";
        var parameters = new List<NpgsqlParameter>
        {
            new("@artistId", artistId)
        };
        findByArtistSql = AppendJoins(findByArtistSql, albumQuerySpecification);
        findByArtistSql += " WHERE \"ArtistId\" = @artistId ORDER BY \"ReleaseDate\", \"Name\"";
        
        _logger.LogDebug(SqlDebugHelper.InterpolateQuery(findByArtistSql, parameters));
        return await context.Albums
            .FromSqlRaw(findByArtistSql, parameters.Cast<object>().ToArray())
            .AsNoTracking()
            .ToListAsync();
    }
    
    public async Task<List<Album>> FindByGenreIdAsync(long genreId, 
        int genreOptions,
        int skip = 0,
        int take = 100,
        IQuerySpecification<Album>? albumQuerySpecification = null)
    {
        const int validMask = GenreOptions.PrimaryGenre | GenreOptions.InfluenceGenre;

        if ((genreOptions & ~validMask) != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(genreOptions),
                $"Invalid genreOptions: {genreOptions}. Must be a combination of GenreOptions.PrimaryGenre (0x1) and/or InfluenceGenre (0x2).");
        }

        var joins = new List<string>();
        var whereConditions = new List<string>();
        var parameters = new List<NpgsqlParameter>()
        {
            new("@genreId", genreId),
            new("@skip", skip),
            new("@take", take),
        };

        if ((genreOptions & GenreOptions.PrimaryGenre) != 0)
        {
            joins.Add(" INNER JOIN \"Album_Genre\" pg ON pg.\"AlbumId\" = a.\"Id\"");
            whereConditions.Add("pg.\"GenreId\" = @genreId");
        }
        
        if ((genreOptions & GenreOptions.InfluenceGenre) != 0)
        {
            joins.Add(" INNER JOIN \"Album_Influence\" ig ON ig.\"AlbumId\" = a.\"Id\"");
            whereConditions.Add("ig.\"GenreId\" = @genreId");
        }
        
        var findByGenreSql = $"""
                              SELECT DISTINCT a.*
                              FROM "Albums" a
                              {string.Join("\n", joins)}
                              WHERE {string.Join(" OR ", whereConditions)}
                              ORDER BY a."ReleaseDate", a."Name"
                              OFFSET @skip LIMIT @take
                              """;
        
        _logger.LogDebug(SqlDebugHelper.InterpolateQuery(findByGenreSql, parameters));
        return await context.Albums
            .FromSqlRaw(findByGenreSql, parameters.Cast<object>().ToArray())
            .AsNoTracking()
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
        var idList = ids.ToArray();
        if (0 == idList.Length)
        {
            return [];
        }
        
        var stringIds = string.Join(",", idList);
        var findInSql = $"SELECT * FROM \"Albums\"";
        findInSql = AppendJoins(findInSql, albumQuerySpecification);
        findInSql += $" WHERE \"Id\" IN ({stringIds}) ORDER BY \"ReleaseDate\", \"Name\"";
        
        _logger.LogDebug($"📜 SQL : {findInSql}");
        return await context.Albums
            .FromSqlRaw(findInSql)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<int> GetCountAsync()
    {
        const string countSql = "SELECT COUNT(*) FROM \"Albums\"";
        _logger.LogDebug($"📜 SQL : {countSql}");
        return await context.Database.ExecuteSqlRawAsync(countSql);
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
                
                // 🧠 Étape 1 : collecter tous les Id de genres référencés
                var allGenreIds = entity.PrimaryGenres.Concat(entity.InfluenceGenres)
                    .Where(g => g.Id != 0)
                    .Select(g => g.Id)
                    .Distinct()
                    .ToList();

                // 🧠 Étape 2 : charger d’un coup les genres suivis
                var trackedGenres = await context.Genres
                    .Where(g => allGenreIds.Contains(g.Id))
                    .ToDictionaryAsync(g => g.Id);

                // 🧼 Étape 3 : remplacer proprement les genres

                existingAlbum.PrimaryGenres.Clear();
                foreach (var g in entity.PrimaryGenres)
                {
                    if (g.Id != 0 && trackedGenres.TryGetValue(g.Id, out var tracked))
                        existingAlbum.PrimaryGenres.Add(tracked);
                    else
                        existingAlbum.PrimaryGenres.Add(g); // genre nouveau ou non trouvé, à ajouter tel quel
                }

                existingAlbum.InfluenceGenres.Clear();
                foreach (var g in entity.InfluenceGenres)
                {
                    if (g.Id != 0 && trackedGenres.TryGetValue(g.Id, out var tracked))
                        existingAlbum.InfluenceGenres.Add(tracked);
                    else
                        existingAlbum.InfluenceGenres.Add(g);
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
            throw new RepositoryException("❌ SAVE ALL Album : Could not persist", ex, _logger);
        }
    }

    private static string AppendJoins(string baseSql, IQuerySpecification<Album>? querySpecification = null)
    {
        if (querySpecification is not AlbumQuerySpecification albumQuerySpecification)
        {
            return baseSql;
        }

        var stringBuilder = new StringBuilder(baseSql);

        if (albumQuerySpecification.IncludeArtist)
        {
            stringBuilder.Append(" INNER JOIN \"Artists\" ON \"Albums\".\"ArtistId\" = \"Artists\".\"Id\"");
        }

        if (albumQuerySpecification.IncludePrimaryGenres)
        {
            stringBuilder.Append(" LEFT JOIN \"Album_Genre\" apg ON \"Albums\".\"Id\" = apg.\"AlbumId\"" +
                                 " LEFT JOIN \"Genres\" pg ON apg.\"GenreId\" = pg.\"Id\"");
        }
        
        if (albumQuerySpecification.IncludeInfluenceGenres)
        {
            stringBuilder.Append(" LEFT JOIN \"Album_Influence\" aig ON \"Albums\".\"Id\" = aig.\"AlbumId\" " +
                                 " LEFT JOIN \"Genres\" ig ON aig.\"GenreId\" = ig.\"Id\"");
        }
        
        return stringBuilder.ToString();
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