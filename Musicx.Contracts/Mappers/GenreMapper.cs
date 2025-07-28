using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;


namespace Musicx.Contracts.Mappers;

public static class GenreMapper
{
    public static OutGenre FromDicoToGenre(this IDictionary<string, object?> genre) => new()
    {
        /*Id = genre.Id,
        
        CreatedAt = genre.CreatedAt,
        UpdatedAt = genre.UpdatedAt,
        
        Parents = genre.Parents.Select(g => new OutGenre { Id = g.Id }).ToList(),
        Children = genre.Children.Select(g => new OutGenre { Id = g.Id }).ToList(),

        Name = genre.Name,
        Description = genre.Description,
        Color = genre.Color,*/
    };

    public static InGenre ToRaw(this OutGenre genre) => new()
    {
        Id = genre.Id,

        ParentIds = genre.Parents.Select(g => g.Id).ToList(),
        ChildIds = genre.Children.Select(g => g.Id).ToList(),

        Name = genre.Name,
        Description = genre.Description,
        Color = genre.Color
    };
}