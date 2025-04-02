using Musicx.Domain.Entities;

namespace Musicx.Application.Common.Interfaces.Persistence;

public interface IQuerySpecification<T>
    where T : BaseEntity;