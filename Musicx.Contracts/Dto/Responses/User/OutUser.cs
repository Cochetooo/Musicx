using Musicx.Contracts.Enums;

namespace Musicx.Contracts.Dto.Responses.User;

public sealed class OutUser : BaseOutputModel
{
    public ICollection<OutRole>? Roles { get; set; }
    
    public string? BannerUrl { get; set; }
    public string? Biography { get; set; }
    public DateTime? BirthDate { get; set; }
    public string Email { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }
    public string? GoogleId { get; set; }
    public string? LastFmUsername { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? PictureUrl { get; set; }
    public bool PrefBannerBlur { get; set; }
    public bool PrefDarkMode { get; set; }
    public RatingMode PrefRatingMode { get; set; }
    public bool PrefShowRatings { get; set; }
    public bool PrefSimpleGenre { get; set; }
}