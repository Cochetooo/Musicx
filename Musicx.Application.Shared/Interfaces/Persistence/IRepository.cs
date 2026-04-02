using System.Linq.Expressions;
using Musicx.Application.Shared.Enums;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;

namespace Musicx.Application.Shared.Interfaces.Persistence;

/// <summary>
/// Base behavior for a repository.
/// </summary>
/// <typeparam name="TIn">An endpoint input class inheriting from BaseInputModel</typeparam>
/// /// <typeparam name="TOut">An endpoint output class inheriting from BaseOutputModel</typeparam>
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
    Task DeleteAsync(long id)
        => throw new NotSupportedException($"{GetType().Name} does not implement delete commands for {typeof(TIn).Name}.");
    
    /// <summary>
    /// Delete a collection of entities.
    /// </summary>
    /// <param name="ids">The unique identifiers of the entities to be deleted</param>
    /// <since>0.6.2</since>
    Task DeleteAllAsync(IEnumerable<long> ids)
        => throw new NotSupportedException($"{GetType().Name} does not implement delete all commands for {typeof(TIn).Name}.");

    /// <summary>
    /// Retrieve an entity by his unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier to search for</param>
    /// <param name="joinSpecification">Gives specific information to what relation objects should be attached.</param>
    /// <returns>An entity if the identifier has been found in the database, else <b>null</b></returns>
    /// <since>0.6.0</since>
    Task<TOut?> FindOneByIdAsync(
        long id, 
        IJoinSpecification<TIn>? joinSpecification = null
    )
        => throw new NotSupportedException($"{GetType().Name} does not implement find one by id queries for {typeof(TIn).Name}.");
    
    /// <summary>
    /// Finds entities with a typed query and returns the items with the computed total.
    /// </summary>
    /// <param name="findQuery">Typed query implementing <see cref="IFindQuery{T}"/>.</param>
    /// <param name="joinSpec">Optional join specification.</param>
    /// <param name="orderSpec">Optional order specification.</param>
    /// <param name="pagingOptions">Optional paging options.</param>
    /// <returns>A generic output list containing items and total count.</returns>
    /// <since>0.7.4</since>
    Task<OutGenericList<TOut>> FindAsync(
        IFindQuery<TIn>? findQuery,
        IJoinSpecification<TIn>? joinSpec = null,
        OrderSpecification<TIn>? orderSpec = null,
        PagingOptions? pagingOptions = null)
        => throw new NotSupportedException($"{GetType().Name} does not implement typed find queries for {typeof(TIn).Name}.");

    /// <summary>
    /// Retrieve entities corresponding to all identifiers prompted.
    /// </summary>
    /// <param name="ids">A list of unique identifiers used to retrieve the entities.</param>
    /// <param name="joinSpec"></param>
    /// <param name="orderSpec"></param>
    /// <returns>A collection of entities as <b>List</b>, empty if no entity matches the IDs.</returns>
    /// <since>0.6.0</since>
    Task<List<TOut>> FindInAsync(
        IEnumerable<long> ids, 
        IJoinSpecification<TIn>? joinSpec = null,
        OrderSpecification<TIn>? orderSpec = null
    )
        => throw new NotSupportedException($"{GetType().Name} does not implement find in array queries for {typeof(TIn).Name}.");
    
    /// <summary>
    /// Counts entities matching a typed query.
    /// </summary>
    /// <param name="findQuery">Typed query used to constrain the count.</param>
    /// <returns>The number of rows matching the query.</returns>
    /// <since>0.7.4</since>
    Task<long> CountAsync(IFindQuery<TIn>? findQuery = null, IJoinSpecification<TIn>? spec = null)
        => throw new NotSupportedException($"{GetType().Name} does not implement global count commands for {typeof(TIn).Name}.");
    
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
    Task<List<long>> SaveAllAsync(IEnumerable<TIn> entities)
        => throw new NotSupportedException($"{GetType().Name} does not implement save all commands for {typeof(TIn).Name}.");
}