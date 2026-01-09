using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Application.Api.Interfaces.Persistence.Repositories.User;

public interface IUserRepository : IRepository<InUser, OutUser>
{
    Task<OutUser?> FindByEmailAsync(string email,
        IQuerySpecification<InUser>? userQuerySpecification = null);
}