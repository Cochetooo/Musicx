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
        
        Parents = genre.ParentIds.Select(g => new OutGenre { Id = g }).ToList(),
        Children = genre.ChildIds.Select(g => new OutGenre { Id = g }).ToList(),

        Name = genre.Name,
        Description = genre.Description,
        Color = genre.Color,
    };

    public static Genre ToEntity(this InGenre genreDto) => new()
    {
        Id = genreDto.Id,
        
        ParentIds = genreDto.ParentIds,
        ChildIds = genreDto.ChildIds,

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
}