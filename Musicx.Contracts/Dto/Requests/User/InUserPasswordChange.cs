namespace Musicx.Contracts.Dto.Requests.User;

public sealed class InUserPasswordChange
{
    public string NewPassword { get; set; } = string.Empty;
}