using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Security;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Application.Api.Interfaces.Persistence.Repositories.Security;

public interface IRoleRepository : IRepository<InRole, OutRole>;