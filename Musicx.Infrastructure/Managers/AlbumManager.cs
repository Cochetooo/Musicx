using System.Linq.Expressions;
using log4net;
using Musicx.Core.Interfaces;
using Musicx.Core.Logging;
using Musicx.Infrastructure.Repositories;
using Musicx.Core.Models;
using Musicx.Data.Entities;
using Musicx.Infrastructure.Caches;
using Musicx.Infrastructure.Helpers;
using Musicx.Infrastructure.Mappers;

namespace Musicx.Infrastructure.Managers;

public interface IAlbumManager : IManager<Album>;

public class AlbumManager(IAlbumRepository albumRepository,
    IAlbumCache albumCache,
    IAlbumMapper albumMapper,
    ILoggerFactory loggerFactory) : IAlbumManager
{
    private readonly ILogger<AlbumManager> Logger = loggerFactory.CreateLogger<AlbumManager>();
    
    /// 🔍 Récupérer un album sous forme de DTO
    public async Task<Album?> FindById(ulong id)
    {
        var entity = await albumRepository.FindById(id);

        if (null == entity)
        {
            return null;
        }
        
        return albumMapper.ToDto(entity);
    }
    
    /// 📜 Récupérer tous les albums sous forme de DTOs
    public async Task<List<Album>> FindAll(int skip = 0, int take = 100,
        Expression<Func<Album, bool>>? filter = null)
    {
        var entityFilter = ExpressionMapper<Album, AlbumEntity>.Convert(filter);
        
        var entities = await albumRepository.FindAll(skip, take, entityFilter);
        
        return entities
            .Select(albumMapper.ToDto)
            .ToList();
    }
    
    public async Task<List<Album>> FindIn(List<ulong> ids)
    {
        var entities = await albumRepository.FindIn(ids);
        
        return entities
            .Select(albumMapper.ToDto)
            .ToList();
    }

    public async Task<Album?> FindExisting(Album album)
    {
        var key = $"{album.Name}|{album.CatalogNumber}";

        var cachedSong = albumCache.Get(key);
        if (null != cachedSong)
        {
            return cachedSong;
        }
        
        var albums = await albumRepository.FindAll(filter:
            a => a.Name == album.Name && a.CatalogNumber == album.CatalogNumber);

        if (0 < albums.Count)
        {
            var existingAlbum = albumMapper.ToDto(albums.First());
            albumCache.Add(key, existingAlbum);
            return existingAlbum;
        }

        return null;
    }

    public async Task<uint> GetCount()
    {
        return await albumRepository.GetCount();
    }

    /// 🆕 Sauvegarder un album à partir d’un DTO
    public async Task<ulong> Save(Album album)
    {
        var entity = albumMapper.ToEntity(album);
        
        return await albumRepository.Save(entity);
    }
    
    /// 🆕 Sauvegarder un album à partir d’un DTO
    public async Task<List<ulong>> SaveAll(IList<Album> albums)
    {
        var entities = new List<AlbumEntity>();
        
        foreach (var album in albums)
        {
            var entity = albumMapper.ToEntity(album);
            entities.Add(entity);
        }
        
        return await albumRepository.SaveAll(entities);
    }
    
    /// ❌ Supprimer un album
    public async Task Delete(ulong id)
    {
        await albumRepository.Delete(id);
    }
}