namespace Musicx.Contracts.Dto.Responses;

public sealed class OutUser : BaseModel
{
    public string Email { get; set; } = null!;
    public bool EmailConfirmed { get; set; }
    public string PasswordHash { get; set; } = null!;
    public string PasswordSalt { get; set; } = null!;
    public string Username { get; set; } = null!;
}