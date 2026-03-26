using Musicx.Contracts.Enums;

namespace Musicx.Contracts.Dto.Requests.User;

public sealed class InUser : BaseInputModel
{
    // Required Columns
    public string Email { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool PrefAutoFollow { get; set; } = true;
    public bool PrefBannerBlur { get; set; } = true;
    public bool PrefDarkMode { get; set; } = true;
    public string PrefLanguage { get; set; } = "en";
    public RatingMode PrefRatingMode { get; set; } = RatingMode.OutOfTen;
    public bool PrefShowRatings { get; set; } = true;
    public bool PrefSimpleGenre { get; set; }
    
    // Optional Relationships
    public IReadOnlyList<long>? RoleIds { get; set; }
    
    // Optional Columns
    public string? BannerUrl { get; set; }
    public string? Biography { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? GoogleId { get; set; }
    public string? LastFmUsername { get; set; }
    public string? Password { get; set; }
    public string? PasswordHash { get; set; }
    public string? PasswordSalt { get; set; }
    public string? PictureUrl { get; set; }
}