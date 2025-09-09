namespace Musicx.Contracts.Dto.Requests.Specifics;

public sealed record SignInRequest(
    string Email,
    string Password
);