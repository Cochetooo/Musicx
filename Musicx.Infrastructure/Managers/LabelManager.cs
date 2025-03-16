using System.Linq.Expressions;
using Musicx.Core.Interfaces;
using Musicx.Core.Logging;
using Musicx.Infrastructure.Repositories;
using Musicx.Core.Models;
using Musicx.Data.Entities;
using Musicx.Infrastructure.Caches;
using Musicx.Infrastructure.Helpers;
using Musicx.Infrastructure.Mappers;

namespace Musicx.Infrastructure.Managers;

public interface ILabelManager : IManager<Label>;

public class LabelManager(ILabelRepository labelRepository, 
    ILabelCache labelCache,
    ILabelMapper labelMapper,
    ILoggerFactory loggerFactory) : ILabelManager
{
    private readonly ILogger<LabelManager> Logger = loggerFactory.CreateLogger<LabelManager>();
    
    /// 🔍 Récupérer un label sous forme de DTO
    public async Task<Label?> FindById(ulong id)
    {
        var entity = await labelRepository.FindById(id);

        if (null == entity)
        {
            return null;
        }
        
        return labelMapper.ToDto(entity);
    }
    
    /// 📜 Récupérer tous les labels sous forme de DTOs
    public async Task<List<Label>> FindAll(int skip = 0, int take = 100,
        Expression<Func<Label, bool>>? filter = null)
    {
        var entityFilter = ExpressionMapper<Label, LabelEntity>.Convert(filter);
        
        var entities = await labelRepository.FindAll(skip, take, entityFilter);
        
        return entities
            .Select(labelMapper.ToDto)
            .ToList();
    }
    
    public async Task<List<Label>> FindIn(List<ulong> ids)
    {
        var entities = await labelRepository.FindIn(ids);
        
        return entities
            .Select(labelMapper.ToDto)
            .ToList();
    }
    
    public async Task<Label?> FindExisting(Label label)
    {
        var key = $"{label.Name}";

        var cachedSong = labelCache.Get(key);
        if (null != cachedSong)
        {
            return cachedSong;
        }
        
        var labels = await labelRepository.FindAll(filter:
            a => a.Name == label.Name);

        if (0 < labels.Count)
        {
            var existingLabel = labelMapper.ToDto(labels.First());
            labelCache.Add(key, existingLabel);
            return existingLabel;
        }

        return null;
    }

    public async Task<uint> GetCount()
    {
        return await labelRepository.GetCount();
    }

    /// 🆕 Sauvegarder un label à partir d’un DTO
    public async Task<ulong> Save(Label label)
    {
        var entity = labelMapper.ToEntity(label);
        
        return await labelRepository.Save(entity);
    }
    
    /// 🆕 Sauvegarder un label à partir d’un DTO
    public async Task<List<ulong>> SaveAll(IList<Label> labels)
    {
        var entities = new List<LabelEntity>();
        
        foreach (var label in labels)
        {
            var entity = labelMapper.ToEntity(label);
            entities.Add(entity);
        }
        
        return await labelRepository.SaveAll(entities);
    }
    
    /// ❌ Supprimer un label
    public async Task Delete(ulong id)
    {
        await labelRepository.Delete(id);
    }
}