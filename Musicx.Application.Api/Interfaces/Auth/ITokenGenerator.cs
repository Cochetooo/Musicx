using Musicx.Contracts.Dto.Responses;

namespace Musicx.Application.Api.Interfaces.Auth;

public interface ITokenGenerator
{
    string Generate(OutUser user);
}