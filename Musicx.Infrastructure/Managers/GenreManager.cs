using System.Linq.Expressions;
using Musicx.Core.Interfaces;
using Musicx.Core.Logging;
using Musicx.Infrastructure.Repositories;
using Musicx.Core.Models;
using Musicx.Data.Entities;
using Musicx.Infrastructure.Caches;
using Musicx.Infrastructure.Helpers;
using Musicx.Infrastructure.Mappers;

namespace Musicx.Infrastructure.Managers;

public interface IGenreManager : IManager<Genre>;

public class GenreManager(IGenreRepository genreRepository, 
    IGenreCache genreCache,
    IGenreMapper genreMapper,
    ILoggerFactory loggerFactory) : IGenreManager
{
    private readonly ILogger<GenreManager> Logger = loggerFactory.CreateLogger<GenreManager>();
    
    /// 🔍 Récupérer un genre sous forme de DTO
    public async Task<Genre?> FindById(ulong id)
    {
        var entity = await genreRepository.FindById(id);

        if (null == entity)
        {
            return null;
        }
        
        return genreMapper.ToDto(entity);
    }
    
    /// 📜 Récupérer tous les genres sous forme de DTOs
    public async Task<List<Genre>> FindAll(int skip = 0, int take = 100,
        Expression<Func<Genre, bool>>? filter = null)
    {
        var entityFilter = ExpressionMapper<Genre, GenreEntity>.Convert(filter);
        
        var entities = await genreRepository.FindAll(skip, take, entityFilter);
        
        return entities
            .Select(genreMapper.ToDto)
            .ToList();
    }
    
    public async Task<List<Genre>> FindIn(List<ulong> ids)
    {
        var entities = await genreRepository.FindIn(ids);
        
        return entities
            .Select(genreMapper.ToDto)
            .ToList();
    }
    
    public async Task<Genre?> FindExisting(Genre genre)
    {
        var key = $"{genre.Name}";

        var cachedSong = genreCache.Get(key);
        if (null != cachedSong)
        {
            return cachedSong;
        }
        
        var genres = await genreRepository.FindAll(filter:
            a => a.Name == genre.Name);

        if (0 < genres.Count)
        {
            var existingGenre = genreMapper.ToDto(genres.First());
            genreCache.Add(key, existingGenre);
            return existingGenre;
        }

        return null;
    }

    public async Task<uint> GetCount()
    {
        return await genreRepository.GetCount();
    }

    /// 🆕 Sauvegarder un genre à partir d’un DTO
    public async Task<ulong> Save(Genre genre)
    {
        var entity = genreMapper.ToEntity(genre);
        
        return await genreRepository.Save(entity);
    }
    
    /// 🆕 Sauvegarder un genre à partir d’un DTO
    public async Task<List<ulong>> SaveAll(IList<Genre> genres)
    {
        var entities = new List<GenreEntity>();
        
        foreach (var genre in genres)
        {
            var entity = genreMapper.ToEntity(genre);
            entities.Add(entity);
        }
        
        return await genreRepository.SaveAll(entities);
    }
    
    /// ❌ Supprimer un genre
    public async Task Delete(ulong id)
    {
        await genreRepository.Delete(id);
    }
}