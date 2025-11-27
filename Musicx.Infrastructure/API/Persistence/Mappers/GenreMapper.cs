using System.Text.Json;
using System.Text.Json.Nodes;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Genres;
using Musicx.Contracts.Enums;
using Musicx.Contracts.Helpers;
using Musicx.Infrastructure.API.Persistence.Columns;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;


namespace Musicx.Infrastructure.API.Persistence.Mappers;

public static class GenreMapper
{
    private static readonly JsonSerializerSettings GenreAliasMapperJsonOptions = new()
    {
        Converters =
        {
            new JsonToOutModelConverter<OutGenreAlias>("genre_alias")
        }
    };
    
    private static readonly JsonSerializerSettings GenreClosureMapperJsonOptions = new()
    {
        Converters =
        {
            new JsonToOutModelConverter<OutGenreClosure>("genre_closure")
        }
    };
    
    private static readonly JsonSerializerSettings GenreRelationMapperJsonOptions = new()
    {
        Converters =
        {
            new JsonToOutModelConverter<OutGenreRelation>("genre_relation")
        }
    };
    
    public static OutGenre FromDicoToGenre(this IDictionary<string, object?> genre, string prefix = "") => new()
    {
        Id = genre.SafeGet<long>(prefix + GenreColumns.Id),
        
        CreatedAt = genre.SafeGet<DateTime>(prefix + GenreColumns.CreatedAt),
        UpdatedAt = genre.SafeGet<DateTime>(prefix + GenreColumns.UpdatedAt),
        
        Aliases = genre.TryGetValue("aliases", out var genreAliasValue)
                        && genreAliasValue is not null
            ? JsonConvert.DeserializeObject<OutGenreAlias[]>(genreAliasValue as string ?? string.Empty,
                GenreAliasMapperJsonOptions)
            : null,
        
        Parents = genre.TryGetValue("parents", out var parentsValue)
                   && parentsValue is not null
            ? JsonConvert.DeserializeObject<GenreClosureNode[]>(parentsValue as string ?? string.Empty,
                GenreClosureMapperJsonOptions) ?? []
            : [],
        
        Children = genre.TryGetValue("children", out var childrenValue)
                  && childrenValue is not null
            ? JsonConvert.DeserializeObject<GenreClosureNode[]>(childrenValue as string ?? string.Empty,
                GenreClosureMapperJsonOptions) ?? []
            : [],

        Relations = genre.TryGetValue("relations", out var genreRelationValue)
                          && genreRelationValue is not null
            ? JsonConvert.DeserializeObject<GenreRelationNode[]>(genreRelationValue as string ?? string.Empty,
                GenreRelationMapperJsonOptions)
            : null,
        
        CanonicalName = genre.SafeGet<string>(prefix + GenreColumns.CanonicalName) ?? "",
        Color = genre.SafeGet<string?>(prefix + GenreColumns.Color),
        Confidence = genre.SafeGet<float?>(prefix + GenreColumns.Confidence),
        CountryOrigin = genre.SafeGet<string?>(prefix + GenreColumns.CountryOrigin),
        Description = genre.SafeGet<string?>(prefix + GenreColumns.Description),
        EraStart = genre.SafeGet<DateTime?>(prefix + GenreColumns.EraStart),
        EraEnd = genre.SafeGet<DateTime?>(prefix + GenreColumns.EraEnd),
        IsVisible = genre.SafeGet<bool>(prefix + GenreColumns.IsVisible),
        Metadata = genre.SafeGet<string?>(prefix + GenreColumns.Metadata),
        ShortName = genre.SafeGet<string?>(prefix + GenreColumns.ShortName),
        IsTaggable = genre.SafeGet<bool>(prefix + GenreColumns.Taggable),
        Type = genre.SafeGet<GenreType>(prefix + GenreColumns.Type),
    };

    public static InGenre ToRaw(this OutGenre genre) => new()
    {
        Id = genre.Id,
        
        IsVisible = genre.IsVisible,
        CanonicalName = genre.CanonicalName,
        Type = genre.Type,
        
        Description = genre.Description,
        Color = genre.Color
    };
}