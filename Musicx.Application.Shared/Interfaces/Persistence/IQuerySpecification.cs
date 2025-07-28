using Musicx.Contracts.Dto.Requests;

namespace Musicx.Application.Shared.Interfaces.Persistence;

/// <summary>
/// Specifications for relationship includes while in a query from a repository.
/// </summary>
/// <typeparam name="T">A base model type</typeparam>
/// <since>0.6.1</since>
public interface IQuerySpecification<T>
    where T : BaseInputModel;