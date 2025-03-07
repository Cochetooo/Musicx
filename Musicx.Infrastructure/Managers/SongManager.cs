using log4net;
using Musicx.Core.Interfaces;
using Musicx.Infrastructure.Repositories;
using Musicx.Entities.Mappers;
using Musicx.Core.Models;
using Musicx.Data.Entities;

namespace Musicx.Infrastructure.Managers;

public class SongManager(SongRepository songRepository) : IManager<Song>
{
    private readonly ILog Logger = LogManager.GetLogger(typeof(SongManager));

    /// 🔍 Récupérer un morceau sous forme de DTO
    public async Task<Song?> GetById(ulong id)
    {
        var entity = await songRepository.GetById(id);
        return entity?.ToDto();
    }
    
    /// 📜 Récupérer tous les morceaux sous forme de DTOs
    public async Task<List<Song>> GetAll(
        int skip = 0, 
        int take = 100,
        string? title = null,
        string? artist = null,
        string? album = null)
    {
        var songEntities = await songRepository.GetAll(skip, take, title, artist, album);
        
        return songEntities
            .Select(se => se.ToDto())
            .OfType<Song>()
            .ToList();
    }
    
    /// 🆕 Sauvegarder un morceau à partir d’un DTO
    public async Task Save(Song song)
    {
        var entity = await songRepository.GetById(song.Id) 
                     ?? new SongEntity();
        
        await songRepository.Save(entity);
    }
    
    /// ❌ Supprimer un morceau
    public async Task Delete(ulong id)
    {
        await songRepository.Delete(id);
    }
}