using log4net;
using Musicx.Core.Interfaces;
using Musicx.Core.Logging;
using Musicx.Core.Models;
using Musicx.Data.Entities;

namespace Musicx.Infrastructure.Mappers;

public interface ILabelMapper : IMapper<Label, LabelEntity>;

public class LabelMapper(ILoggerFactory loggerFactory) : ILabelMapper
{
    private readonly ILogger<LabelMapper> Logger = loggerFactory.CreateLogger<LabelMapper>();
    
    // Mappage de l'entité vers le DTO
    public Label ToDto(LabelEntity labelEntity)
    {
        var labelDto = new Label
        {
            Id = labelEntity.Id,
            Description = labelEntity.Description,
            Name = labelEntity.Name
        };

        // Si vous avez une relation avec plusieurs albums, vous pouvez mapper les IDs des albums dans le DTO
        //labelDto.AlbumIds = labelEntity.Albums.Select(a => a.Id).ToList();

        return labelDto;
    }

    // Mappage du DTO vers l'entité
    public LabelEntity ToEntity(Label labelDto)
    {
        var entity = new LabelEntity
        {
            Id = labelDto.Id,
            Description = labelDto.Description,
            Name = labelDto.Name
        };

        Logger.Warn("⚠️ Albums set to []. Please fix later.");
        //entity.Albums = labelDto.AlbumIds.Select(albumId => new AlbumEntity { Id = albumId }).ToList();

        return entity;
    }
}
