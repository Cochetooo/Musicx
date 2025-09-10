using Musicx.Contracts.Dto.Requests.Specifics;

namespace Musicx.Application.Web.Interfaces.UseCases.Specifics;

public interface IAuthSignInUseCase
{
    Task<string?> ExecuteAsync(SignInRequest request);
    string? Execute(SignInRequest request);
}