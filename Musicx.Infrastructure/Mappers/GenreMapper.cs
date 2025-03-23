using log4net;
using Musicx.Core.Interfaces;
using Musicx.Core.Logging;
using Musicx.Core.Models;
using Musicx.Data.Entities;
using Musicx.Infrastructure.Loaders;
using Musicx.Infrastructure.Managers;

namespace Musicx.Infrastructure.Mappers;

public interface IGenreMapper : IMapper<Genre, GenreEntity>;

public class GenreMapper(ILoggerFactory loggerFactory) : IGenreMapper
{
    private readonly ILogger<GenreMapper> Logger = loggerFactory.CreateLogger<GenreMapper>();

    // Mappage de l'entité vers le DTO
    public Genre ToDto(GenreEntity genreEntity)
    {
        var genreDto = new Genre
        {
            Id = genreEntity.Id,
            Name = genreEntity.Name
        };

        // Mappage des Parents et des Children en termes d'IDs
        genreDto.ParentIds = genreEntity.Parents.Select(p => p.ParentId).ToList();
        genreDto.ChildIds = genreEntity.Children.Select(c => c.ChildId).ToList();

        return genreDto;
    }

    // Mappage du DTO vers l'entité
    public GenreEntity ToEntity(Genre genreDto)
    {
        var entity = new GenreEntity
        {
            Id = genreDto.Id,
            Name = genreDto.Name
        };

        // Mappage des Parents et des Children (en créant les relations Many-to-Many)
        entity.Parents = genreDto.ParentIds.Select(parentId => new GenreParentEntity
        {
            ParentId = parentId,
            ChildId = entity.Id  // Relier cet enfant à ce parent
        }).ToList();

        entity.Children = genreDto.ChildIds.Select(childId => new GenreParentEntity
        {
            ParentId = entity.Id,  // Relier ce parent à cet enfant
            ChildId = childId
        }).ToList();

        return entity;
    }
}
