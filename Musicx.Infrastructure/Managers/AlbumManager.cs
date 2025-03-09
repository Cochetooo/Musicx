using System.Linq.Expressions;
using log4net;
using Musicx.Core.Interfaces;
using Musicx.Core.Logging;
using Musicx.Infrastructure.Repositories;
using Musicx.Core.Models;
using Musicx.Data.Entities;
using Musicx.Infrastructure.Helpers;
using Musicx.Infrastructure.Mappers;

namespace Musicx.Infrastructure.Managers;

public interface IAlbumManager : IManager<Album>;

public class AlbumManager(IAlbumRepository albumRepository, ILoggerFactory loggerFactory) : IAlbumManager
{
    private readonly ILogger Logger = loggerFactory.CreateLogger(typeof(AlbumManager));
    
    /// 🔍 Récupérer un album sous forme de DTO
    public async Task<Album?> FindById(ulong id)
    {
        var entity = await albumRepository.FindById(id);
        
        return entity?.ToDto();
    }
    
    /// 📜 Récupérer tous les albums sous forme de DTOs
    public async Task<List<Album>> FindAll(int skip = 0, int take = 100,
        Expression<Func<Album, bool>>? filter = null)
    {
        var entityFilter = ExpressionMapper<Album, AlbumEntity>.Convert(filter);
        
        var entities = await albumRepository.FindAll(skip, take, entityFilter);
        
        return entities
            .Select(a => a.ToDto())
            .OfType<Album>()
            .ToList();
    }

    /// 🆕 Sauvegarder un album à partir d’un DTO
    public async Task Save(Album album)
    {
        var entity = new AlbumEntity();
        entity.FromDto(album);
        
        await albumRepository.Save(entity);
    }
    
    /// 🆕 Sauvegarder un album à partir d’un DTO
    public async Task SaveAll(IList<Album> albums)
    {
        var entities = new List<AlbumEntity>();
        
        foreach (var album in albums)
        {
            var entity = new AlbumEntity();
            entity.FromDto(album);
            entities.Add(entity);
        }
        
        await albumRepository.SaveAll(entities);
    }
    
    /// ❌ Supprimer un album
    public async Task Delete(ulong id)
    {
        await albumRepository.Delete(id);
    }
}