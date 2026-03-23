namespace Musicx.Infrastructure.API.Persistence.Columns.Artist;

public static class ArtistColumns
{
    public const string Id = "artist_id";
    
    public const string CreatedAt = "artist_created_at";
    public const string UpdatedAt = "artist_updated_at";

    public const string Alias = "artist_alias";
    public const string ArtworkUrl = "artist_artwork_url";
    public const string CalculatedGenres = "artist_calculated_genres";
    public const string CalculatedInfluences = "artist_calculated_influences";
    public const string CalculatedGenreCounts = "artist_calculated_genre_counts";
    public const string CalculatedInfluenceCounts = "artist_calculated_influence_counts";
    public const string CalculatedDescriptorCounts = "artist_calculated_descriptor_counts";
    public const string CalculatedSceneCounts = "artist_calculated_scene_counts";
    public const string CalculatedMovementCounts = "artist_calculated_movement_counts";
    public const string CurrentCountry = "artist_current_country";
    public const string CurrentRegion = "artist_current_region";
    public const string CurrentTown = "artist_current_town";
    public const string Description = "artist_description";
    public const string IsVisible = "artist_is_visible";
    public const string Name = "artist_name";
    public const string OriginCountry = "artist_origin_country";
    public const string OriginRegion = "artist_origin_region";
    public const string OriginTown = "artist_origin_town";
    
    public const string Discriminator = "artist_discriminator";
    
    public const string FormationDate = "artist_formation_date";
    public const string SplitDate = "artist_split_date";
    
    public const string FirstName = "artist_first_name";
    public const string LastName = "artist_last_name";
    public const string BirthDate = "artist_birth_date";
    public const string DeathDate = "artist_death_date";
}