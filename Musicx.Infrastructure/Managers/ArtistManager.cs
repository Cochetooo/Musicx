using log4net;
using Musicx.Core.Interfaces;
using Musicx.Infrastructure.Repositories;
using Musicx.Entities.Mappers;
using Musicx.Core.Models;
using Musicx.Data.Entities;

namespace Musicx.Infrastructure.Managers;

public class ArtistManager(ArtistRepository artistRepository) : IManager<Artist>
{
    private readonly ILog Logger = LogManager.GetLogger(typeof(ArtistManager));
    
    /// 🔍 Récupérer un artiste sous forme de DTO
    public async Task<Artist?> GetById(ulong id)
    {
        var entity = await artistRepository.GetById(id);
        return entity?.ToDto();
    }

    /// 📜 Récupérer tous les artistes sous forme de DTOs
    public async Task<List<Artist>> GetAll(int skip = 0, int take = 100)
    {
        var entities = await artistRepository.GetAll(skip, take);
        
        return entities
            .Select(a => a.ToDto())
            .OfType<Artist>()
            .ToList();
    }

    /// 🆕 Sauvegarder un artiste à partir d’un DTO
    public async Task Save(Artist artist)
    {
        var entity = await artistRepository.GetById(artist.Id);

        if (null == entity)
        {
            entity = CreateEntityFromDto(artist);
        }
        
        await artistRepository.Save(entity);
    }
    
    /// ❌ Supprimer un artiste
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