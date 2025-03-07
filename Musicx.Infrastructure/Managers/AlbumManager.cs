using System.Linq.Expressions;
using log4net;
using Musicx.Core.Interfaces;
using Musicx.Core.Logging;
using Musicx.Infrastructure.Repositories;
using Musicx.Core.Models;
using Musicx.Data.Entities;
using Musicx.Infrastructure.Mappers;

namespace Musicx.Infrastructure.Managers;

public class AlbumManager(AlbumRepository albumRepository, ILoggerFactory loggerFactory) : IManager<Album>
{
    private readonly ILogger Logger = loggerFactory.CreateLogger(typeof(AlbumManager));
    
    /// 🔍 Récupérer un album sous forme de DTO
    public async Task<Album?> FindById(ulong id)
    {
        var entity = await albumRepository.FindById(id);
        
        return entity?.ToDto();
    }
    
    /// 📜 Récupérer tous les albums sous forme de DTOs
    public async Task<IEnumerable<Album>> FindAll(int skip = 0, int take = 100,
        Expression<Func<Album, bool>>? filter = null)
    {
        var entities = await albumRepository.FindAll(skip, take, filter);
        
        return entities
            .Select(a => a.ToDto())
            .OfType<Album>()
            .ToList();
    }

    /// 🆕 Sauvegarder un album à partir d’un DTO
    public async Task Save(Album album)
    {
        var entity = await albumRepository.FindById(album.Id)
            ?? new AlbumEntity();
        
        await albumRepository.Save(entity);
    }
    
    /// 🆕 Sauvegarder un album à partir d’un DTO
    public async Task SaveAll(IList<Album> albums)
    {
        var entities = await albumRepository
            .FindAll(filter: a => albums.Any(b => b.Id == a.Id));
        
        await albumRepository.SaveAll(entities);
    }
    
    /// ❌ Supprimer un album
    public async Task Delete(ulong id)
    {
        await albumRepository.Delete(id);
    }
}