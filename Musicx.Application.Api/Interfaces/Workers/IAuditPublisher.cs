using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Security;

namespace Musicx.Application.Api.Interfaces.Workers;

public interface IAuditPublisher
{
    Task PublishAsync(InAuditLog evt);
}