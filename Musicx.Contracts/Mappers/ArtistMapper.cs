using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Domain.Models;

namespace Musicx.Contracts.Mappers;

public static class ArtistMapper
{
    public static OutArtist ToDto(this Artist artist)
    {
        var artistDto = new OutArtist
        {
            Id = artist.Id,

            Name = artist.Name,
            ArtworkUrl = artist.ArtworkUrl,
            Country = artist.Country,
        };

        switch (artist)
        {
            case PersonArtist personArtist:
                artistDto.Discriminator = "Person";
                
                artistDto.Bands = personArtist.Bands.Select(b => new OutArtist { Id = b.Id, Name = b.Name }).ToList();
                artistDto.BirthDate = personArtist.BirthDate;
                artistDto.DeathDate = personArtist.DeathDate;
                break;
            case BandArtist bandArtist:
                artistDto.Discriminator = "Band";
                
                artistDto.Members = bandArtist.Members.Select(m => new OutArtist { Id = m.Id, Name = m.Name }).ToList();
                artistDto.FormationDate = bandArtist.FormationDate;
                artistDto.SplitDate = bandArtist.SplitDate;
                break;
            default:
                artistDto.Discriminator = "Artist";
                break;
        }
        
        return artistDto;
    }
    
    public static Artist ToEntity(this InArtist artistDto)
    {
        return artistDto.Discriminator switch
        {
            "Artist" => new Artist
            {
                Id = artistDto.Id,
                Name = artistDto.Name,
                ArtworkUrl = artistDto.ArtworkUrl,
                Country = artistDto.Country
            },
            "Band" => new BandArtist
            {
                Id = artistDto.Id,
                Name = artistDto.Name,
                ArtworkUrl = artistDto.ArtworkUrl,
                Country = artistDto.Country,
                Members = artistDto.MemberIds!.Select(m => new PersonArtist { Id = m }).ToList(),
                FormationDate = artistDto.FormationDate,
                SplitDate = artistDto.SplitDate
            },
            "Person" => new PersonArtist
            {
                Id = artistDto.Id,
                Name = artistDto.Name,
                ArtworkUrl = artistDto.ArtworkUrl,
                Country = artistDto.Country,
                Bands = artistDto.BandIds!.Select(m => new BandArtist { Id = m }).ToList(),
                BirthDate = artistDto.BirthDate,
                DeathDate = artistDto.DeathDate
            },
            _ => throw new InvalidCastException("Invalid artist type")
        };
    }
}