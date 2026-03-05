using Musicx.Application.Api.Models.Auth;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.User;

namespace Musicx.Application.Api.Interfaces.Persistence.Repositories.User;

public interface IUserRepository : IRepository<InUser, OutUser>
{
    Task<OutUser?> FindByEmailAsync(string email,
        IJoinSpecification<InUser>? joinSpec = null);

    Task<OutUserAuth?> FindAuthByEmailAsync(string email,
        IJoinSpecification<InUser>? joinSpec = null);
}