using log4net;
using Musicx.Core.Models;
using Musicx.Data.Entities;

namespace Musicx.Infrastructure.Mappers;

public static class LabelMapper
{
    private static readonly ILog Logger = LogManager.GetLogger(typeof(LabelMapper));
    
    // Mappage de l'entité vers le DTO
    public static Label? ToDto(this LabelEntity? labelEntity)
    {
        if (null == labelEntity)
        {
            Logger.Warn("⚠️ Entity is null.");
            return null;
        }
        
        var labelDto = new Label
        {
            Id = labelEntity.Id,
            Description = labelEntity.Description,
            Name = labelEntity.Name
        };

        // Si vous avez une relation avec plusieurs albums, vous pouvez mapper les IDs des albums dans le DTO
        labelDto.AlbumIds = labelEntity.Albums.Select(a => a.Id).ToList();

        return labelDto;
    }

    // Mappage du DTO vers l'entité
    public static void FromDto(this LabelEntity entity, Label labelDto)
    {
        entity.Id = labelDto.Id;
        
        entity.Description = labelDto.Description;
        entity.Name = labelDto.Name;
        
        Logger.Warn("⚠️ Albums set to []. Please fix later.");
        entity.Albums = [];
        //entity.Albums = labelDto.AlbumIds.Select(albumId => new AlbumEntity { Id = albumId }).ToList();
    }
}
