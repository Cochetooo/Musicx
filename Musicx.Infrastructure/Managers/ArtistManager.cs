using System.Linq.Expressions;
using Musicx.Core.Interfaces;
using Musicx.Core.Logging;
using Musicx.Infrastructure.Repositories;
using Musicx.Core.Models;
using Musicx.Data.Entities;
using Musicx.Infrastructure.Helpers;
using Musicx.Infrastructure.Mappers;

namespace Musicx.Infrastructure.Managers;

public class ArtistManager(ArtistRepository artistRepository, ILoggerFactory loggerFactory) : IManager<Artist>
{
    private readonly ILogger Logger = loggerFactory.CreateLogger(typeof(ArtistManager));
    
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

    /// 🆕 Sauvegarder un artist à partir d’un DTO
    public async Task Save(Artist artist)
    {
        var entity = await artistRepository.FindById(artist.Id);

        if (null == entity)
        {
            entity = CreateEntityFromDto(artist);
        }
        
        await artistRepository.Save(entity);
    }
    
    /// 🆕 Sauvegarder un artist à partir d’un DTO
    public async Task SaveAll(IList<Artist> artists)
    {
        var entities = await artistRepository
            .FindAll(filter: a => artists.Any(b => b.Id == a.Id));
        
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