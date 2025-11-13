using Musicx.Contracts.Enums;

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
    public string? Biography { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? GoogleId { get; set; }
    public string? LastFmUsername { get; set; }
    public string? Password { get; set; }
    public string? PasswordHash { get; set; }
    public string? PasswordSalt { get; set; }
    public bool? PrefDarkMode { get; set; }
    public RatingMode? PrefRatingMode { get; set; }
    public bool? PrefShowRatings { get; set; }
    public bool? PrefSimpleGenre { get; set; }
    public string? PictureUrl { get; set; }
}