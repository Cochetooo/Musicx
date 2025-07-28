using Musicx.Contracts.Dto.Enums;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;


namespace Musicx.Contracts.Mappers;

public static class ArtistMapper
{
    public static OutArtist ToDto(this Artist artist)
    {
        var artistDto = new OutArtist
        {
            Id = artist.Id,
            
            CreatedAt = artist.CreatedAt,
            UpdatedAt = artist.UpdatedAt,

            Name = artist.Name,
            ArtworkUrl = artist.ArtworkUrl,
            Country = artist.Country,
        };

        switch (artist)
        {
            case PersonArtist personArtist:
                artistDto.Discriminator = ArtistDiscriminator.Person;
                
                artistDto.Bands = personArtist.Bands.Select(b => new OutArtist { Id = b.Id, Name = b.Name }).ToList();
                artistDto.FirstName = personArtist.FirstName;
                artistDto.LastName = personArtist.LastName;
                artistDto.BirthDate = personArtist.BirthDate;
                artistDto.DeathDate = personArtist.DeathDate;
                break;
            case BandArtist bandArtist:
                artistDto.Discriminator = ArtistDiscriminator.Band;
                
                artistDto.Members = bandArtist.Members.Select(m => new OutArtist { Id = m.Id, Name = m.Name }).ToList();
                artistDto.FormationDate = bandArtist.FormationDate;
                artistDto.SplitDate = bandArtist.SplitDate;
                break;
            default:
                artistDto.Discriminator = ArtistDiscriminator.Artist;
                break;
        }
        
        return artistDto;
    }
    
    public static Artist ToEntity(this InArtist artistDto)
    {
        return artistDto.Discriminator switch
        {
            ArtistDiscriminator.Artist => new Artist
            {
                Id = artistDto.Id,
                Name = artistDto.Name,
                ArtworkUrl = artistDto.ArtworkUrl,
                Country = artistDto.Country
            },
            ArtistDiscriminator.Band => new BandArtist
            {
                Id = artistDto.Id,
                Name = artistDto.Name,
                ArtworkUrl = artistDto.ArtworkUrl,
                Country = artistDto.Country,
                Members = artistDto.MemberIds!.Select(m => new PersonArtist { Id = m }).ToList(),
                FormationDate = artistDto.FormationDate,
                SplitDate = artistDto.SplitDate
            },
            ArtistDiscriminator.Person => new PersonArtist
            {
                Id = artistDto.Id,
                Name = artistDto.Name,
                ArtworkUrl = artistDto.ArtworkUrl,
                Country = artistDto.Country,
                Bands = artistDto.BandIds!.Select(m => new BandArtist { Id = m }).ToList(),
                FirstName = artistDto.FirstName,
                LastName = artistDto.LastName,
                BirthDate = artistDto.BirthDate,
                DeathDate = artistDto.DeathDate
            },
            _ => throw new InvalidCastException("Invalid artist type")
        };
    }

    public static InArtist ToRaw(this OutArtist artist) => new()
    {
        Id = artist.Id,
        Name = artist.Name,
        ArtworkUrl = artist.ArtworkUrl,
        Country = artist.Country,
        Discriminator = artist.Discriminator,
        BandIds = artist.Bands.Select(b => b.Id).ToList(),
        MemberIds = artist.Members.Select(m => m.Id).ToList(),
        FormationDate = artist.FormationDate,
        SplitDate = artist.SplitDate,
        FirstName = artist.FirstName,
        LastName = artist.LastName,
        BirthDate = artist.BirthDate,
        DeathDate = artist.DeathDate
    };
}