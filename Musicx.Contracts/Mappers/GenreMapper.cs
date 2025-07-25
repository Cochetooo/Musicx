using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Domain.Models;

namespace Musicx.Contracts.Mappers;

public static class GenreMapper
{
    public static OutGenre ToDto(this Genre genre) => new()
    {
        Id = genre.Id,
        
        CreatedAt = genre.CreatedAt,
        UpdatedAt = genre.UpdatedAt,
        
        Parents = genre.Parents.Select(g => new OutGenre { Id = g.Id }).ToList(),
        Children = genre.Children.Select(g => new OutGenre { Id = g.Id }).ToList(),

        Name = genre.Name,
        Description = genre.Description,
        Color = genre.Color,
    };

    public static Genre ToEntity(this InGenre genreDto) => new()
    {
        Id = genreDto.Id,
        
        Parents = genreDto.ParentIds.Select(Proxy).ToList(),
        Children = genreDto.ChildIds.Select(Proxy).ToList(),

        Name = genreDto.Name,
        Description = genreDto.Description,
        Color = genreDto.Color
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

    private static Genre Proxy(long id) => new()
    {
        Id = id,
        Name = string.Empty
    };
}