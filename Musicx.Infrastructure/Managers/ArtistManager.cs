using System.Linq.Expressions;
using Musicx.Core.Interfaces;
using Musicx.Core.Logging;
using Musicx.Infrastructure.Repositories;
using Musicx.Core.Models;
using Musicx.Data.Entities;
using Musicx.Data.Mappers;
using Musicx.Infrastructure.Caches;
using Musicx.Infrastructure.Helpers;

namespace Musicx.Infrastructure.Managers;

public interface IArtistManager : IManager<Artist>;

public class ArtistManager(IArtistRepository artistRepository, 
    IArtistCache artistCache,
    ILoggerFactory loggerFactory) : IArtistManager
{
    private readonly ILogger<ArtistManager> Logger = loggerFactory.CreateLogger<ArtistManager>();
    
    /// 🔍 Récupérer un artist sous forme de DTO
    public async Task<Artist?> FindById(ulong id)
    {
        var entity = await artistRepository.FindById(id);
        
        return entity?.ToDto();
    }
    
    /// 📜 Récupérer tous les artists sous forme de DTOs
    public async Task<List<Artist>> FindAll(int skip = 0, int take = 100,
        Expression<Func<Artist, bool>>? filter = null)
    {
        var entityFilter = ExpressionMapper<Artist, ArtistEntity>.Convert(filter);
        
        var entities = await artistRepository.FindAll(skip, take, entityFilter);
        
        return entities
            .Select(a => a.ToDto())
            .OfType<Artist>()
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
        
        var existingArtist = artists.FirstOrDefault().ToDto();
        if (null != existingArtist)
        {
            artistCache.Add(key, existingArtist);
        }
        
        return existingArtist;
    }

    public async Task<uint> GetCount()
    {
        return await artistRepository.GetCount();
    }

    /// 🆕 Sauvegarder un artist à partir d’un DTO
    public async Task Save(Artist artist)
    {
        var entity = CreateEntityFromDto(artist);
        entity.FromDto(artist);
        
        await artistRepository.Save(entity);
    }
    
    /// 🆕 Sauvegarder un artist à partir d’un DTO
    public async Task SaveAll(IList<Artist> artists)
    {
        var entities = new List<ArtistEntity>();
        
        foreach (var artist in artists)
        {
            var entity = CreateEntityFromDto(artist);
            entity.FromDto(artist);
            entities.Add(entity);
        }
        
        await artistRepository.SaveAll(entities);
    }
    
    /// ❌ Supprimer un artist
    public async Task Delete(ulong id)
    {
        await artistRepository.Delete(id);
    }

    private ArtistEntity CreateEntityFromDto(Artist artist)
    {
        if (artist is BandArtist)
        {
            return new BandArtistEntity();
        }
        
        if (artist is PersonArtist)
        {
            return new PersonArtistEntity();
        }

        Logger.Error($"❌ Artist DTO is of type {artist.GetType().Name}! Cannot create entity.");
        throw new InvalidCastException();
    }
}