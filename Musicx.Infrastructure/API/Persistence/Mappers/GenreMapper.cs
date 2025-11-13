using System.Text.Json;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Enums;
using Musicx.Contracts.Helpers;
using Musicx.Infrastructure.API.Persistence.Columns;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;


namespace Musicx.Infrastructure.API.Persistence.Mappers;

public static class GenreMapper
{
    private static readonly JsonSerializerSettings GenreMapperJsonOptions = new()
    {
        Converters =
        {
            new JsonToOutModelConverter<OutGenre>("genre")
        }
    };
    
    public static OutGenre FromDicoToGenre(this IDictionary<string, object?> genre) => new()
    {
        Id = genre.SafeGet<long>(GenreColumns.Id),
        
        CreatedAt = genre.SafeGet<DateTime>(GenreColumns.CreatedAt),
        UpdatedAt = genre.SafeGet<DateTime>(GenreColumns.UpdatedAt),
        
        Parents = genre.TryGetValue("parents", out var parentValue)
                && parentValue is not null
            ? JsonConvert.DeserializeObject<OutGenre[]>(parentValue as string ?? string.Empty,
                GenreMapperJsonOptions)
            : null,
        
        Children = genre.TryGetValue("children", out var childValue)
                && childValue is not null
            ? JsonConvert.DeserializeObject<OutGenre[]>(childValue as string ?? string.Empty,
                GenreMapperJsonOptions)
            : null,
        
        Color = genre.SafeGet<string>(GenreColumns.Color),
        Description = genre.SafeGet<string>(GenreColumns.Description),
        IsVisible = genre.SafeGet<bool>(GenreColumns.IsVisible),
        Name = genre.SafeGet<string>(GenreColumns.Name) ?? "",
        Type = genre.SafeGet<GenreType>(GenreColumns.Type),
    };

    public static InGenre ToRaw(this OutGenre genre) => new()
    {
        Id = genre.Id,

        ParentIds = genre.Parents?.Select(g => g.Id).ToList(),
        ChildIds = genre.Children?.Select(g => g.Id).ToList(),
        
        IsVisible = genre.IsVisible,
        Name = genre.Name,
        Type = genre.Type,
        
        Description = genre.Description,
        Color = genre.Color
    };
}