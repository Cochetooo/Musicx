using log4net;
using Musicx.Core.Models;
using Musicx.Data.Entities;

namespace Musicx.Data.Mappers;

public static class ArtistMapper
{
    public static readonly ILog Logger = LogManager.GetLogger(typeof(ArtistMapper));
    
    public static Artist ToDto(this ArtistEntity entity)
    {
        switch (entity)
        {
            case BandArtistEntity bandArtistEntity:
            {
                // On récupère les identifiants des relations (Lazy Loading pour les relations Many-to-Many)
                var memberIds = bandArtistEntity.Members?.Select(g => g.PersonId).ToList() ?? new List<ulong>();
        
                return new BandArtist()
                {
                    Id = bandArtistEntity.Id,
            
                    ArtworkUrl = bandArtistEntity.ArtworkUrl,
                    Country = bandArtistEntity.Country,
                    FormationDate = bandArtistEntity.FormationDate,
                    MemberIds = memberIds,
                    Name = bandArtistEntity.Name,
                    SplitDate = bandArtistEntity.SplitDate
                };
            }
            case PersonArtistEntity personArtistEntity:
            {
                // On récupère les identifiants des relations (Lazy Loading pour les relations Many-to-Many)
                var bandIds = personArtistEntity.Bands?.Select(g => g.BandId).ToList() ?? new List<ulong>();
        
                return new PersonArtist()
                {
                    Id = personArtistEntity.Id,
            
                    ArtworkUrl = personArtistEntity.ArtworkUrl,
                    BandIds = bandIds,
                    BirthDate = personArtistEntity.BirthDate,
                    Country = personArtistEntity.Country,
                    DeathDate = personArtistEntity.DeathDate,
                    FirstName = personArtistEntity.FirstName,
                    Name = personArtistEntity.Name,
                };
            }
            default:
                Logger.Warn($"⚠️ Entity is not inheriting BandArtistEntity or PersonArtistEntity: {entity.GetType().Name}");
                throw new InvalidCastException();
        }
    }
    
    public static void FromDto(this ArtistEntity entity, Artist dto) 
    {
        entity.Id = dto.Id;
        
        entity.ArtworkUrl = dto.ArtworkUrl;
        entity.Country = dto.Country;
        entity.Name = dto.Name;

        if (dto is PersonArtist personDto && entity is PersonArtistEntity personArtistEntity)
        {
            personArtistEntity.Bands = [];
            
            personArtistEntity.BirthDate = personDto.BirthDate;
            personArtistEntity.DeathDate = personDto.DeathDate;
            personArtistEntity.FirstName = personDto.FirstName;
            
            personArtistEntity.Discriminator = "Person";
        }
        else if (dto is BandArtist bandDto && entity is BandArtistEntity bandArtistEntity)
        {
            bandArtistEntity.Members = [];
            
            bandArtistEntity.SplitDate = bandDto.SplitDate;
            bandArtistEntity.FormationDate = bandDto.FormationDate;

            bandArtistEntity.Discriminator = "Band";
        }
        else
        {
            Logger.Error($"❌ Entity ({entity.GetType().Name}) and DTO ({dto.GetType().Name}) not compatible!");
        }
    }
}