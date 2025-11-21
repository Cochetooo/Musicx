namespace Musicx.Infrastructure.API.Persistence.Columns;

public static class GenreRelationColumns
{
    public const string FromGenreId = "genre_relation_from_genre_id";
    public const string ToGenreId = "genre_relation_to_genre_id";
    
    public const string CreatedAt = "genre_relation_created_at";
    public const string UpdatedAt = "genre_relation_updated_at";
    
    public const string Metadata = "genre_relation_metadata";
    public const string Type = "genre_relation_type";
    public const string Weight = "genre_relation_weight";
}