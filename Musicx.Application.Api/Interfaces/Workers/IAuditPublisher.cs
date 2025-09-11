using Musicx.Contracts.Dto.Requests;

namespace Musicx.Application.Api.Interfaces.Workers;

public interface IAuditPublisher
{
    Task PublishAsync(InAuditLog evt);
}