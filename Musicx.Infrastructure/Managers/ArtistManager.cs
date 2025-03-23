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

public interface IArtistManager : IManager<Artist>;

public class ArtistManager(IArtistRepository artistRepository, 
    IArtistCache artistCache,
    IArtistMapper artistMapper,
    ILoggerFactory loggerFactory) : IArtistManager
{
    private readonly ILogger<ArtistManager> Logger = loggerFactory.CreateLogger<ArtistManager>();

    /// 🔍 Récupérer un artist sous forme de DTO
    public async Task<Artist?> FindById(ulong id)
    {
        var entity = await artistRepository.FindById(id);

        if (null == entity)
        {
            return null;
        }
        
        return artistMapper.ToDto(entity);
    }

    /// 📜 Récupérer tous les artists sous forme de DTOs
    public async Task<List<Artist>> FindAll(int skip = 0, int take = 100,
        Expression<Func<Artist, bool>>? filter = null)
    {
        var entityFilter = ExpressionMapper<Artist, ArtistEntity>.Convert(filter);
        
        var entities = await artistRepository.FindAll(skip, take, entityFilter);
        
        return entities
            .Select(artistMapper.ToDto)
            .ToList();
    }

    public async Task<List<Artist>> FindIn(List<ulong> ids)
    {
        var entities = await artistRepository.FindIn(ids);
        
        return entities
            .Select(artistMapper.ToDto)
            .ToList();
    }

    public async Task<Artist?> FindExisting(Artist artist)
    {
        var key = $"{artist.Name}";

        var cachedSong = artistCache.Get(key);
        if (null != cachedSong)
        {
            return cachedSong;
        }
        
        var artists = await artistRepository.FindAll(filter:
            a => a.Name == artist.Name);

        if (0 < artists.Count)
        {
            var existingArtist = artistMapper.ToDto(artists.First());
            artistCache.Add(key, existingArtist);
            return existingArtist;
        }

        return null;
    }

    public async Task<uint> GetCount()
    {
        return await artistRepository.GetCount();
    }

    /// 🆕 Sauvegarder un artist à partir d’un DTO
    public async Task<ulong> Save(Artist artist)
    {
        var entity = artistMapper.ToEntity(artist);
        
        return await artistRepository.Save(entity);
    }

    /// 🆕 Sauvegarder un artist à partir d’un DTO
    public async Task<List<ulong>> SaveAll(IList<Artist> artists)
    {
        var entities = new List<ArtistEntity>();
        
        foreach (var artist in artists)
        {
            var entity = artistMapper.ToEntity(artist);
            entities.Add(entity);
        }
        
        return await artistRepository.SaveAll(entities);
    }

    /// ❌ Supprimer un artist
    public async Task Delete(ulong id)
    {
        await artistRepository.Delete(id);
    }
}