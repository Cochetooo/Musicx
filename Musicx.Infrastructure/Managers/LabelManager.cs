using System.Linq.Expressions;
using Musicx.Core.Interfaces;
using Musicx.Core.Logging;
using Musicx.Infrastructure.Repositories;
using Musicx.Core.Models;
using Musicx.Data.Entities;
using Musicx.Infrastructure.Helpers;
using Musicx.Infrastructure.Mappers;

namespace Musicx.Infrastructure.Managers;

public class LabelManager(LabelRepository labelRepository, ILoggerFactory loggerFactory) : IManager<Label>
{
    private readonly ILogger Logger = loggerFactory.CreateLogger(typeof(LabelManager));
    
    /// 🔍 Récupérer un label sous forme de DTO
    public async Task<Label?> FindById(ulong id)
    {
        var entity = await labelRepository.FindById(id);
        
        return entity?.ToDto();
    }
    
    /// 📜 Récupérer tous les labels sous forme de DTOs
    public async Task<List<Label>> FindAll(int skip = 0, int take = 100,
        Expression<Func<Label, bool>>? filter = null)
    {
        var entityFilter = ExpressionMapper<Label, LabelEntity>.Convert(filter);
        
        var entities = await labelRepository.FindAll(skip, take, entityFilter);
        
        return entities
            .Select(a => a.ToDto())
            .OfType<Label>()
            .ToList();
    }

    /// 🆕 Sauvegarder un label à partir d’un DTO
    public async Task Save(Label label)
    {
        var entity = await labelRepository.FindById(label.Id)
                     ?? new LabelEntity();
        
        await labelRepository.Save(entity);
    }
    
    /// 🆕 Sauvegarder un label à partir d’un DTO
    public async Task SaveAll(IList<Label> labels)
    {
        var entities = await labelRepository
            .FindAll(filter: a => labels.Any(b => b.Id == a.Id));
        
        await labelRepository.SaveAll(entities);
    }
    
    /// ❌ Supprimer un label
    public async Task Delete(ulong id)
    {
        await labelRepository.Delete(id);
    }
}