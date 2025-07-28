using Musicx.Contracts.Dto.Enums;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;


namespace Musicx.Contracts.Mappers;

public static class ArtistMapper
{
    public static OutArtist FromDicoToArtist(this IDictionary<string, object?> artist)
    {
        /*var artistDto = new OutArtist
        {
            Id = artist.Id,
            
            CreatedAt = artist.CreatedAt,
            UpdatedAt = artist.UpdatedAt,

            Name = artist.Name,
            ArtworkUrl = artist.ArtworkUrl,
            Country = artist.Country,
        };*/

        /*switch (artist)
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
        }*/
        
        return new OutArtist();
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