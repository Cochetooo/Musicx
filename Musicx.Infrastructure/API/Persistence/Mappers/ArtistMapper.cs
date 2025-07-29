using Musicx.Contracts.Dto.Enums;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Helpers;
using Musicx.Infrastructure.API.Persistence.Columns;


namespace Musicx.Infrastructure.API.Persistence.Mappers;

public static class ArtistMapper
{
    public static OutArtist FromDicoToArtist(this IDictionary<string, object?> artist) => new()
    {
        Id = artist.SafeGet<long>(ArtistColumns.Id),

        CreatedAt = artist.SafeGet<DateTime>(ArtistColumns.CreatedAt),
        UpdatedAt = artist.SafeGet<DateTime>(ArtistColumns.UpdatedAt),

        ArtworkUrl = artist.SafeGet<string>(ArtistColumns.ArtworkUrl),
        Country = artist.SafeGet<string>(ArtistColumns.Country),
        Description = artist.SafeGet<string>(ArtistColumns.Description),
        Name = artist.SafeGet<string>(ArtistColumns.Name) ?? "",
        Region = artist.SafeGet<string>(ArtistColumns.Region),
        Town = artist.SafeGet<string>(ArtistColumns.Town),

        Discriminator = artist.SafeGet<ArtistDiscriminator>(ArtistColumns.Discriminator),

        FormationDate = artist.SafeGet<DateTime?>(ArtistColumns.FormationDate),
        SplitDate = artist.SafeGet<DateTime?>(ArtistColumns.SplitDate),

        FirstName = artist.SafeGet<string>(ArtistColumns.FirstName),
        LastName = artist.SafeGet<string>(ArtistColumns.LastName),
        BirthDate = artist.SafeGet<DateTime?>(ArtistColumns.BirthDate),
        DeathDate = artist.SafeGet<DateTime?>(ArtistColumns.DeathDate),
    };

    public static InArtist ToRaw(this OutArtist artist) => new()
    {
        Id = artist.Id,
        
        ArtworkUrl = artist.ArtworkUrl,
        Country = artist.Country,
        Description = artist.Description,
        Name = artist.Name,
        Region = artist.Region,
        Town = artist.Town,

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