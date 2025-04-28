using Musicx.Domain.Models;

namespace Musicx.Application.Shared.Interfaces.Persistence;

public interface IQuerySpecification<T>
    where T : BaseModel;