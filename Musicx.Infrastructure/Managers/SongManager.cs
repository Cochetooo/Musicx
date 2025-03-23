using System.Linq.Expressions;
using Musicx.Core.Interfaces;
using Musicx.Core.Logging;
using Musicx.Core.Models;
using Musicx.Data.Entities;
using Musicx.Infrastructure.Caches;
using Musicx.Infrastructure.Helpers;
using Musicx.Infrastructure.Mappers;
using Musicx.Infrastructure.Repositories;

namespace Musicx.Infrastructure.Managers;

public interface ISongManager : IManager<Song>;

public class SongManager(ISongRepository songRepository, 
    ISongCache songCache,
    ISongMapper songMapper,
    ILoggerFactory loggerFactory) : ISongManager
{
    private readonly ILogger<SongManager> Logger = loggerFactory.CreateLogger<SongManager>();

    /// 🔍 Récupérer un song sous forme de DTO
    public async Task<Song?> FindById(ulong id)
    {
        var entity = await songRepository.FindById(id);

        if (null == entity)
        {
            return null;
        }
        
        return songMapper.ToDto(entity);
    }

    /// 📜 Récupérer tous les songs sous forme de DTOs
    public async Task<List<Song>> FindAll(int skip = 0, int take = 100,
        Expression<Func<Song, bool>>? filter = null)
    {
        var entityFilter = ExpressionMapper<Song, SongEntity>.Convert(filter);
        
        var entities = await songRepository.FindAll(skip, take, entityFilter);
        
        return entities
            .Select(songMapper.ToDto)
            .ToList();
    }

    public async Task<List<Song>> FindIn(List<ulong> ids)
    {
        var entities = await songRepository.FindIn(ids);
        
        return entities
            .Select(songMapper.ToDto)
            .ToList();
    }

    public async Task<Song?> FindExisting(Song song)
    {
        var key = $"{song.Title}|{song.Duration}|{song.TrackNumber}";

        var cachedSong = songCache.Get(key);
        if (null != cachedSong)
        {
            return cachedSong;
        }
        
        var songs = await songRepository.FindAll(filter:
            a => a.Title == song.Title && a.Duration == song.Duration && a.TrackNumber == song.TrackNumber);

        if (0 < songs.Count)
        {
            var existingSong = songMapper.ToDto(songs.First());
            songCache.Add(key, existingSong);
            return existingSong;
        }

        return null;
    }

    public async Task<uint> GetCount()
    {
        return await songRepository.GetCount();
    }

    /// 🆕 Sauvegarder un song à partir d’un DTO
    public async Task<ulong> Save(Song song)
    {
        var entity = songMapper.ToEntity(song);
        
        return await songRepository.Save(entity);
    }

    /// 🆕 Sauvegarder un song à partir d’un DTO
    public async Task<List<ulong>> SaveAll(IList<Song> songs)
    {
        var entities = new List<SongEntity>();
        
        foreach (var song in songs)
        {
            var entity = songMapper.ToEntity(song);
            entities.Add(entity);
        }
        
        return await songRepository.SaveAll(entities);
    }

    /// ❌ Supprimer un song
    public async Task Delete(ulong id)
    {
        await songRepository.Delete(id);
    }
}