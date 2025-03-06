using log4net;
using MusicxApi.Models;

namespace Musicx.Entities.Mappers;

public static class ArtistMapper
{
    public static readonly ILog Logger = LogManager.GetLogger(typeof(ArtistMapper));
    
    public static BandArtist? ToDto(this BandArtistEntity? entity)
    {
        if (null == entity)
        {
            Logger.Warn("⚠️ Entity is null.");
            return null;
        }
        
        return new()
        {
            Id = entity.Id,
            
            ArtworkUrl = entity.ArtworkUrl,
            Country = entity.Country,
            FormationDate = entity.FormationDate,
            MemberIds = entity.MemberIds,
            Name = entity.Name,
            SplitDate = entity.SplitDate
        };
    }
    
    public static BandArtistEntity? ToEntity(this BandArtist? artist)
    {
        if (null == artist)
        {
            Logger.Warn("⚠️ Entity is null.");
            return null;
        }
        
        return new()
        {
            Id = artist.Id,
            
            ArtworkUrl = artist.ArtworkUrl,
            Country = artist.Country,
            FormationDate = artist.FormationDate,
            MemberIds = artist.MemberIds,
            Name = artist.Name,
            SplitDate = artist.SplitDate,
            
            Discriminator = "Band"
        };
    }

    public static PersonArtist? ToDto(this PersonArtistEntity? entity)
    {
        if (null == entity)
        {
            Logger.Warn("⚠️ Entity is null.");
            return null;
        }
        
        return new()
        {
            Id = entity.Id,
            
            ArtworkUrl = entity.ArtworkUrl,
            BandIds = entity.BandIds,
            BirthDate = entity.BirthDate,
            Country = entity.Country,
            DeathDate = entity.DeathDate,
            FirstName = entity.FirstName,
            Name = entity.Name,
        };
    }

    public static PersonArtistEntity? ToEntity(this PersonArtist? artist)
    {
        if (null == artist)
        {
            Logger.Warn("⚠️ Entity is null.");
            return null;
        }
        
        return new()
        {
            Id = artist.Id,
            
            ArtworkUrl = artist.ArtworkUrl,
            BandIds = artist.BandIds,
            BirthDate = artist.BirthDate,
            Country = artist.Country,
            DeathDate = artist.DeathDate,
            FirstName = artist.FirstName,
            Name = artist.Name,
            
            Discriminator = "Person",
        };
    }
}