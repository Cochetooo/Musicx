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
    ILoggerFactory loggerFactory) : ISongManager
{
    private readonly ILogger<SongManager> Logger = loggerFactory.CreateLogger<SongManager>();
    
    /// 🔍 Récupérer un song sous forme de DTO
    public async Task<Song?> FindById(ulong id)
    {
        var entity = await songRepository.FindById(id);
        
        return entity?.ToDto();
    }
    
    /// 📜 Récupérer tous les songs sous forme de DTOs
    public async Task<List<Song>> FindAll(int skip = 0, int take = 100,
        Expression<Func<Song, bool>>? filter = null)
    {
        var entityFilter = ExpressionMapper<Song, SongEntity>.Convert(filter);
        
        var entities = await songRepository.FindAll(skip, take, entityFilter);
        
        return entities
            .Select(a => a.ToDto())
            .OfType<Song>()
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
        
        var existingSong = songs.FirstOrDefault().ToDto();
        if (null != existingSong)
        {
            songCache.Add(key, existingSong);
        }
        
        return existingSong;
    }

    public async Task<uint> GetCount()
    {
        return await songRepository.GetCount();
    }

    /// 🆕 Sauvegarder un song à partir d’un DTO
    public async Task Save(Song song)
    {
        var entity = new SongEntity();
        entity.FromDto(song);
        
        await songRepository.Save(entity);
    }
    
    /// 🆕 Sauvegarder un song à partir d’un DTO
    public async Task SaveAll(IList<Song> songs)
    {
        var entities = new List<SongEntity>();
        
        foreach (var song in songs)
        {
            var entity = new SongEntity();
            entity.FromDto(song);
            entities.Add(entity);
        }
        
        await songRepository.SaveAll(entities);
    }
    
    /// ❌ Supprimer un song
    public async Task Delete(ulong id)
    {
        await songRepository.Delete(id);
    }
}