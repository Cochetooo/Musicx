using Musicx.Contracts.Enums;

namespace Musicx.Contracts.Dto.Responses.Artist;

public sealed class OutArtist : BaseOutputModel
{
    public string? Alias { get; set; }
    public string? ArtworkUrl { get; set; }
    public string? CalculatedGenres { get; set; }
    public string? CalculatedInfluences { get; set; }
    public string? CalculatedGenreCounts { get; set; }
    public string? CalculatedInfluenceCounts { get; set; }
    public string? CalculatedDescriptorCounts { get; set; }
    public string? CalculatedSceneCounts { get; set; }
    public string? CalculatedMovementCounts { get; set; }
    public string? CurrentCountry { get; set; }
    public string? CurrentRegion { get; set; }
    public string? CurrentTown { get; set; }
    public string? Description { get; set; }
    public ArtistDiscriminator Discriminator { get; set; }
    public bool IsVisible { get; set; }
    public string Name { get; set; } = null!;
    public string? OriginCountry { get; set; }
    public string? OriginRegion { get; set; }
    public string? OriginTown { get; set; }
    public OutArtistRatingStat? Stats { get; set; }
    public string? WikipediaUrl { get; set; }

    public IReadOnlyList<OutArtist> Members { get; set; } = [];
    public DateTime? FormationDate { get; set; }
    public DateTime? SplitDate { get; set; }

    public IReadOnlyList<OutArtist> Bands { get; set; } = [];
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime? DeathDate { get; set; }
}