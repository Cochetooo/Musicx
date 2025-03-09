using System.Linq.Expressions;
using Musicx.Core.Interfaces;
using Musicx.Core.Logging;
using Musicx.Infrastructure.Repositories;
using Musicx.Core.Models;
using Musicx.Data.Entities;
using Musicx.Infrastructure.Helpers;
using Musicx.Infrastructure.Mappers;

namespace Musicx.Infrastructure.Managers;

public interface IGenreManager : IManager<Genre>;

public class GenreManager(IGenreRepository genreRepository, ILoggerFactory loggerFactory) : IGenreManager
{
    private readonly ILogger Logger = loggerFactory.CreateLogger(typeof(GenreManager));
    
    /// 🔍 Récupérer un genre sous forme de DTO
    public async Task<Genre?> FindById(ulong id)
    {
        var entity = await genreRepository.FindById(id);
        
        return entity?.ToDto();
    }
    
    /// 📜 Récupérer tous les genres sous forme de DTOs
    public async Task<List<Genre>> FindAll(int skip = 0, int take = 100,
        Expression<Func<Genre, bool>>? filter = null)
    {
        var entityFilter = ExpressionMapper<Genre, GenreEntity>.Convert(filter);
        
        var entities = await genreRepository.FindAll(skip, take, entityFilter);
        
        return entities
            .Select(a => a.ToDto())
            .OfType<Genre>()
            .ToList();
    }

    /// 🆕 Sauvegarder un genre à partir d’un DTO
    public async Task Save(Genre genre)
    {
        var entity = new GenreEntity();
        entity.FromDto(genre);
        
        await genreRepository.Save(entity);
    }
    
    /// 🆕 Sauvegarder un genre à partir d’un DTO
    public async Task SaveAll(IList<Genre> genres)
    {
        var entities = new List<GenreEntity>();
        
        foreach (var genre in genres)
        {
            var entity = new GenreEntity();
            entity.FromDto(genre);
            entities.Add(entity);
        }
        
        await genreRepository.SaveAll(entities);
    }
    
    /// ❌ Supprimer un genre
    public async Task Delete(ulong id)
    {
        await genreRepository.Delete(id);
    }
}