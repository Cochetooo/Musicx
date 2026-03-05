using Musicx.Contracts.Dto.Responses.User;

namespace Musicx.Application.Api.Models.Auth;

public sealed class OutUserAuth
{
    public OutUser User { get; set; } = new();
    public string? PasswordHash { get; set; }
    public string? PasswordSalt { get; set; }
}