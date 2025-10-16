namespace Musicx.Infrastructure.API.Persistence.Columns;

public static class ArtistColumns
{
    public const string Id = "artist_id";
    
    public const string CreatedAt = "artist_created_at";
    public const string UpdatedAt = "artist_updated_at";

    public const string Alias = "artist_alias";
    public const string ArtworkUrl = "artist_artwork_url";
    public const string CalculatedGenres = "artist_calculated_genres";
    public const string CalculatedInfluences = "artist_calculated_influences";
    public const string Country = "artist_country";
    public const string Description = "artist_description";
    public const string Name = "artist_name";
    public const string Region = "artist_region";
    public const string Town = "artist_town";
    
    public const string Discriminator = "artist_discriminator";
    
    public const string FormationDate = "artist_formation_date";
    public const string SplitDate = "artist_split_date";
    
    public const string FirstName = "artist_first_name";
    public const string LastName = "artist_last_name";
    public const string BirthDate = "artist_birth_date";
    public const string DeathDate = "artist_death_date";
}