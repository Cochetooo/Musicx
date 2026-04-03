using Musicx.Contracts.Dto.Requests.Specifics;

namespace Musicx.Application.Shared.Interfaces.UseCases.Security;

public interface IAuthSignInService
{
    Task<string?> ExecuteAsync(SignInRequest request);
    string? Execute(SignInRequest request);
}