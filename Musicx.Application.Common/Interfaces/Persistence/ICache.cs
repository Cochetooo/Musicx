using Musicx.Domain.Entities;

namespace Musicx.Application.Common.Interfaces.Persistence;

/// <summary>
/// Base behavior for an entity cache.
/// </summary>
/// <typeparam name="T">An entity</typeparam>
/// <since>0.6.0</since>
public interface ICache<T>
    where T : BaseEntity
{
    /// <summary>
    /// Get an entity from the cache.
    /// </summary>
    /// <param name="key">The key to find an entity</param>
    /// <returns>The entity with that key</returns>
    /// <since>0.6.0</since>
    T? Get(string key);
    
    /// <summary>
    /// Add an entity to the cache.
    /// </summary>
    /// <param name="key">The key to find this entity</param>
    /// <param name="entity">The entity to put to the cache</param>
    /// <since>0.6.0</since>
    void Add(string key, T entity);
}