using log4net;
using Musicx.Core.Interfaces;
using Musicx.Infrastructure.Repositories;
using Musicx.Entities.Mappers;
using Musicx.Core.Models;
using Musicx.Data.Entities;

namespace Musicx.Infrastructure.Managers;

public class GenreManager(GenreRepository genreRepository) : IManager<Genre>
{
    private readonly ILog Logger = LogManager.GetLogger(typeof(GenreManager));

    /// 🔍 Récupérer un genre sous forme de DTO
    public async Task<Genre?> GetById(ulong id)
    {
        var entity = await genreRepository.GetById(id);
        return entity?.ToDto();
    }
    
    /// 📜 Récupérer tous les genres sous forme de DTOs
    public async Task<List<Genre>> GetAll(int skip = 0, int take = 100)
    {
        var entities = await genreRepository.GetAll(skip, take);
        
        return entities
            .Select(se => se.ToDto())
            .OfType<Genre>()
            .ToList();
    }
    
    /// 🆕 Sauvegarder un genre à partir d’un DTO
    public async Task Save(Genre genre)
    {
        var entity = await genreRepository.GetById(genre.Id) 
                     ?? new GenreEntity();
        
        await genreRepository.Save(entity);
    }
    
    /// ❌ Supprimer un genre
    public async Task Delete(ulong id)
    {
        await genreRepository.Delete(id);
    }
}