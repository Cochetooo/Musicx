using System.Linq.Expressions;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Application.Shared.Interfaces.Persistence;

/// <summary>
/// Base behavior for a repository.
/// </summary>
/// <typeparam name="T">A class inheriting from BaseEntity</typeparam>
/// <since>0.6.0</since>
public interface IRepository<TIn, TOut>
    where TIn : BaseInputModel
    where TOut : BaseOutputModel
{
    /// <summary>
    /// Delete an entity.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <since>0.6.0</since>
    Task DeleteAsync(long id);
    
    /// <summary>
    /// Delete a collection of entities.
    /// </summary>
    /// <param name="ids">The unique identifiers of the entities to be deleted</param>
    /// <since>0.6.2</since>
    Task DeleteAllAsync(IEnumerable<long> ids);

    /// <summary>
    /// Retrieve an entity by his unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier to search for</param>
    /// <param name="songQuerySpecification">Gives specific information to what relation objects should be attached.</param>
    /// <returns>An entity if the identifier has been found in the database, else <b>null</b></returns>
    /// <since>0.6.0</since>
    Task<TOut?> FindByIdAsync(long id, IQuerySpecification<TIn>? songQuerySpecification = null);
    
    /// <summary>
    /// Retrieve all entities that matches filter criteria, or all entities if no filter is specified.
    /// </summary>
    /// <param name="skip">Offset when retrieving all rows, useful for pagination.</param>
    /// <param name="take">Maximum number of rows taken, prevents response from being too large.</param>
    /// <param name="filterExact">Use equality for the filter instead of a similarity algorithm</param>
    /// <param name="filterSimilitude">The similarity rate for the algorithm to find similar results.</param>
    /// <param name="filter">An expression that entities must match to be in the result.</param>
    /// <param name="songQuerySpecification">Gives specific information to what relation objects should be attached.</param>
    /// <param name="order">Sort the collection according to table columns</param>
    /// <returns>A collection of entities as <b>List</b>, empty if no entity has been found matching filter criteria.</returns>
    /// <since>0.6.0</since>
    Task<List<TOut>> FindAsync(
        long skip = 0, 
        long take = 100, 
        bool? filterExact = null,
        double? filterSimilitude = 0.4,
        string? filter = null,
        string? order = null,
        IQuerySpecification<TIn>? songQuerySpecification = null
    );

    /// <summary>
    /// Retrieve entities corresponding to all identifiers prompted.
    /// </summary>
    /// <param name="ids">A list of unique identifiers used to retrieve the entities.</param>
    /// <param name="songQuerySpecification">Gives specific information to what relation objects should be attached.</param>
    /// <returns>A collection of entities as <b>List</b>, empty if no entity matches the IDs.</returns>
    /// <since>0.6.0</since>
    Task<List<TOut>> FindIn(IEnumerable<long> ids, IQuerySpecification<TIn>? songQuerySpecification = null);
    
    /// <summary>
    /// Get the number of entities in this table.
    /// </summary>
    /// <since>0.6.0</since>
    Task<long> GetCountAsync();
    
    /// <summary>
    /// Create or update an entity depending on the ID being set or not.
    /// </summary>
    /// <param name="entity">The entity to be persisted</param>
    /// <returns>The id of the entity persisted (can be useful when creating and wanting to retrieve the ID afterwards)</returns>
    /// <since>0.6.0</since>
    Task<long> SaveAsync(TIn entity);

    /// <summary>
    /// Create or update a collection of entities depending on the ID being set or not for each.
    /// </summary>
    /// <param name="entities">The entities to be persisted</param>
    /// <returns>A list of the ids of the entities persisted (can be useful when creating and wanting to retrieve the IDs afterwards)</returns>
    /// <since>0.6.0</since>
    Task<List<long>> SaveAllAsync(IEnumerable<TIn> entities);
}