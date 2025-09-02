namespace Musicx.Contracts.Dto.Responses;

public sealed class OutUser : BaseOutputModel
{
    public ICollection<OutRole>? Roles { get; set; }
    
    public string Email { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }
    public string? GoogleId { get; set; }
    public string? LastFmUsername { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? PasswordHash { get; set; }
    public string? PasswordSalt { get; set; }
}