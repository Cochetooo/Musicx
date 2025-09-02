namespace Musicx.Contracts.Dto.Requests;

public sealed class InUser : BaseInputModel
{
    // Required Columns
    public string Name { get; set; } = string.Empty;
    
    // Optional Columns
    public string? GoogleId { get; set; }
    public string? LastFmUsername { get; set; }
    public string? PasswordHash { get; set; }
}