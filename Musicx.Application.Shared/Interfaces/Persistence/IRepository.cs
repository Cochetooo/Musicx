using System.Linq.Expressions;
using Musicx.Domain.Models;

namespace Musicx.Application.Shared.Interfaces.Persistence;

/// <summary>
/// Base behavior for a repository.
/// </summary>
/// <typeparam name="T">A class inheriting from BaseEntity</typeparam>
/// <since>0.6.0</since>
public interface IRepository<T>
    where T : BaseModel
{
    /// <summary>
    /// Delete an entity.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <since>0.6.0</since>
    Task DeleteAsync(long id);

    /// <summary>
    /// Retrieve an entity by his unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier to search for</param>
    /// <param name="songQuerySpecification">Gives specific information to what relation objects should be attached.</param>
    /// <returns>An entity if the identifier has been found in the database, else <b>null</b></returns>
    /// <since>0.6.0</since>
    Task<T?> FindByIdAsync(long id, IQuerySpecification<T>? songQuerySpecification = null);
    
    /// <summary>
    /// Retrieve all entities that matches filter criteria, or all entities if no filter is specified.
    /// </summary>
    /// <param name="skip">Offset when retrieving all rows, useful for pagination.</param>
    /// <param name="take">Maximum number of rows taken, prevents response from being too large.</param>
    /// <param name="filter">An expression that entities must match to be in the result.</param>
    /// <param name="songQuerySpecification">Gives specific information to what relation objects should be attached.</param>
    /// <returns>A collection of entities as <b>List</b>, empty if no entity has been found matching filter criteria.</returns>
    /// <since>0.6.0</since>
    Task<List<T>> FindAsync(
        int skip = 0, 
        int take = 100, 
        Expression<Func<T, bool>>? filter = null,
        IQuerySpecification<T>? songQuerySpecification = null);

    /// <summary>
    /// Retrieve entities corresponding to all identifiers prompted.
    /// </summary>
    /// <param name="ids">A list of unique identifiers used to retrieve the entities.</param>
    /// <param name="songQuerySpecification">Gives specific information to what relation objects should be attached.</param>
    /// <returns>A collection of entities as <b>List</b>, empty if no entity matches the IDs.</returns>
    /// <since>0.6.0</since>
    Task<List<T>> FindIn(IEnumerable<long> ids, IQuerySpecification<T>? songQuerySpecification = null);
    
    /// <summary>
    /// Get the number of entities in this table.
    /// </summary>
    /// <since>0.6.0</since>
    Task<int> GetCountAsync();
    
    /// <summary>
    /// Create or update an entity depending on the ID being set or not.
    /// </summary>
    /// <param name="entity">The entity to be persisted</param>
    /// <returns>The id of the entity persisted (can be useful when creating and wanting to retrieve the ID afterwards)</returns>
    /// <since>0.6.0</since>
    Task<long> SaveAsync(T entity);

    /// <summary>
    /// Create or update a collection of entities depending on the ID being set or not for each.
    /// </summary>
    /// <param name="entities">The entities to be persisted</param>
    /// <returns>A list of the ids of the entities persisted (can be useful when creating and wanting to retrieve the IDs afterwards)</returns>
    /// <since>0.6.0</since>
    Task<List<long>> SaveAllAsync(IEnumerable<T> entities);
}