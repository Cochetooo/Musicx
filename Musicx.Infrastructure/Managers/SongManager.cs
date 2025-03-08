using System.Linq.Expressions;
using Musicx.Core.Interfaces;
using Musicx.Core.Logging;
using Musicx.Core.Models;
using Musicx.Data.Entities;
using Musicx.Infrastructure.Helpers;
using Musicx.Infrastructure.Mappers;
using Musicx.Infrastructure.Repositories;

namespace Musicx.Infrastructure.Managers;

public class SongManager(SongRepository songRepository, ILoggerFactory loggerFactory) : IManager<Song>
{
    private readonly ILogger Logger = loggerFactory.CreateLogger(typeof(SongManager));
    
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

    /// 🆕 Sauvegarder un song à partir d’un DTO
    public async Task Save(Song song)
    {
        var entity = await songRepository.FindById(song.Id)
                     ?? new SongEntity();
        
        await songRepository.Save(entity);
    }
    
    /// 🆕 Sauvegarder un song à partir d’un DTO
    public async Task SaveAll(IList<Song> songs)
    {
        var entities = await songRepository
            .FindAll(filter: a => songs.Any(b => b.Id == a.Id));
        
        await songRepository.SaveAll(entities);
    }
    
    /// ❌ Supprimer un song
    public async Task Delete(ulong id)
    {
        await songRepository.Delete(id);
    }
}