using log4net;
using MusicxApi.Models;

namespace Musicx.Entities.Mappers;

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
    public static LabelEntity? ToEntity(this Label? labelDto)
    {
        if (null == labelDto)
        {
            Logger.Warn("⚠️ Entity is null.");
            return null;
        }
        
        var labelEntity = new LabelEntity
        {
            Id = labelDto.Id,
            Description = labelDto.Description,
            Name = labelDto.Name
        };

        // Si vous avez une relation avec les albums, vous pouvez utiliser les AlbumIds pour les lier à cette entité
        // Mais étant donné qu'il s'agit d'une relation One-to-Many, il vous faut un traitement spécifique
        // pour charger et lier les albums si nécessaire (par exemple, les albums peuvent être liés par un service
        // ou une autre approche de gestion de la persistance).
        //labelEntity.Albums = labelDto.AlbumIds.Select(albumId => new AlbumEntity { Id = albumId }).ToList();

        return labelEntity;
    }
}
