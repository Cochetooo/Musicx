namespace Musicx.Contracts.Dto.Requests;

public sealed class InUser : BaseInputModel
{
    // Required Columns
    public string Email { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }
    public string Name { get; set; } = string.Empty;
    
    // Optional Relationships
    public IReadOnlyList<long>? RoleIds { get; set; }
    
    // Optional Columns
    public string? GoogleId { get; set; }
    public string? LastFmUsername { get; set; }
    public string? PasswordHash { get; set; }
    public string? PasswordSalt { get; set; }
}