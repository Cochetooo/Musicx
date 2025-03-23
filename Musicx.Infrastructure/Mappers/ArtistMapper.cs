using log4net;
using Musicx.Core.Interfaces;
using Musicx.Core.Logging;
using Musicx.Core.Models;
using Musicx.Data.Entities;
using Musicx.Infrastructure.Loaders;
using Musicx.Infrastructure.Managers;

namespace Musicx.Infrastructure.Mappers;

public interface IArtistMapper : IMapper<Artist, ArtistEntity>;

public class ArtistMapper(ILoggerFactory loggerFactory) : IArtistMapper
{
    private readonly ILogger<ArtistMapper> Logger = loggerFactory.CreateLogger<ArtistMapper>();

    public Artist ToDto(ArtistEntity entity)
    {
        switch (entity)
        {
            case BandArtistEntity bandArtistEntity:
            {
                // On récupère les identifiants des relations (Lazy Loading pour les relations Many-to-Many)
                var memberIds = bandArtistEntity.Members?.Select(g => g.PersonId).ToList() ?? new List<ulong>();
        
                var bandArtist = new BandArtist
                {
                    Id = bandArtistEntity.Id,
            
                    ArtworkUrl = bandArtistEntity.ArtworkUrl,
                    Country = bandArtistEntity.Country,
                    FormationDate = bandArtistEntity.FormationDate,
                    MemberIds = memberIds,
                    Name = bandArtistEntity.Name,
                    SplitDate = bandArtistEntity.SplitDate
                };
                
                return bandArtist;
            }
            case PersonArtistEntity personArtistEntity:
            {
                // On récupère les identifiants des relations (Lazy Loading pour les relations Many-to-Many)
                var bandIds = personArtistEntity.Bands?.Select(g => g.BandId).ToList() ?? new List<ulong>();
        
                var personArtist = new PersonArtist
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

                return personArtist;
            }
            default:
                Logger.Warn($"⚠️ Entity is not inheriting BandArtistEntity or PersonArtistEntity: {entity.GetType().Name}");
                throw new InvalidCastException();
        }
    }

    public ArtistEntity ToEntity(Artist dto)
    {
        ArtistEntity entity;

        if (dto is PersonArtist personDto)
        {
            entity = new PersonArtistEntity();
            
            var personEntity = (PersonArtistEntity)entity;
            
            personEntity.BirthDate = personDto.BirthDate;
            personEntity.DeathDate = personDto.DeathDate;
            personEntity.FirstName = personDto.FirstName;
            
            personEntity.Discriminator = "Person";
        }
        else if (dto is BandArtist bandDto)
        {
            entity = new BandArtistEntity();
            
            var bandEntity = (BandArtistEntity)entity;
            bandEntity.SplitDate = bandDto.SplitDate;
            bandEntity.FormationDate = bandDto.FormationDate;

            bandEntity.Discriminator = "Band";
        }
        else
        {
            Logger.Error($"❌ DTO ({dto.GetType().Name}) not compatible!");
            throw new InvalidCastException();
        }
                
        entity.Id = dto.Id;
        
        entity.ArtworkUrl = dto.ArtworkUrl;
        entity.Country = dto.Country;
        entity.Name = dto.Name;

        return entity;
    }
}