using System.Text.Json;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Helpers;
using Musicx.Infrastructure.API.Persistence.Columns;


namespace Musicx.Infrastructure.API.Persistence.Mappers;

public static class GenreMapper
{
    private static readonly JsonSerializerOptions GenreMapperJsonOptions = new()
    {
        PropertyNamingPolicy = new DbToOutModelPolicy("genre"),
        PropertyNameCaseInsensitive = true,
    };
    
    public static OutGenre FromDicoToGenre(this IDictionary<string, object?> genre) => new()
    {
        Id = genre.SafeGet<long>(GenreColumns.Id),
        
        CreatedAt = genre.SafeGet<DateTime>(GenreColumns.CreatedAt),
        UpdatedAt = genre.SafeGet<DateTime>(GenreColumns.UpdatedAt),
        
        Parents = genre.TryGetValue("parents", out var parentValue)
                && parentValue is JsonElement pvJson
            ? JsonSerializer.Deserialize<OutGenre[]>(pvJson.GetRawText(),
                GenreMapperJsonOptions)
            : null,
        
        Children = genre.TryGetValue("children", out var childValue)
                && childValue is JsonElement cvJson
            ? JsonSerializer.Deserialize<OutGenre[]>(cvJson.GetRawText(),
                GenreMapperJsonOptions)
            : null,
        
        Color = genre.SafeGet<string>(GenreColumns.Color),
        Description = genre.SafeGet<string>(GenreColumns.Description),
        Name = genre.SafeGet<string>(GenreColumns.Name) ?? "",
    };

    public static InGenre ToRaw(this OutGenre genre) => new()
    {
        Id = genre.Id,

        ParentIds = genre.Parents?.Select(g => g.Id).ToList(),
        ChildIds = genre.Children?.Select(g => g.Id).ToList(),

        Name = genre.Name,
        Description = genre.Description,
        Color = genre.Color
    };
}