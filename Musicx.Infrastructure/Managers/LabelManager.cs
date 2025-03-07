using log4net;
using Musicx.Core.Interfaces;
using Musicx.Infrastructure.Repositories;
using Musicx.Entities.Mappers;
using Musicx.Core.Models;
using Musicx.Data.Entities;

namespace Musicx.Infrastructure.Managers;

public class LabelManager(LabelRepository labelRepository) : IManager<Label>
{
    private readonly ILog Logger = LogManager.GetLogger(typeof(LabelManager));

    /// 🔍 Récupérer un label sous forme de DTO
    public async Task<Label?> GetById(ulong id)
    {
        var entity = await labelRepository.GetById(id);
        return entity?.ToDto();
    }
    
    /// 📜 Récupérer tous les labels sous forme de DTOs
    public async Task<List<Label>> GetAll(int skip = 0, int take = 100)
    {
        var entities = await labelRepository.GetAll(skip, take);
        
        return entities
            .Select(se => se.ToDto())
            .OfType<Label>()
            .ToList();
    }
    
    /// 🆕 Sauvegarder un label à partir d’un DTO
    public async Task Save(Label label)
    {
        var entity = await labelRepository.GetById(label.Id) 
                     ?? new LabelEntity();
        
        await labelRepository.Save(entity);
    }
    
    /// ❌ Supprimer un label
    public async Task Delete(ulong id)
    {
        await labelRepository.Delete(id);
    }
}