using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Enums;
using Musicx.Application.Shared.Helpers;
using Musicx.Infrastructure.API.Persistence.Columns;


namespace Musicx.Infrastructure.API.Persistence.Mappers;

public static class ArtistMapper
{
    public static OutArtist FromDicoToArtist(this IDictionary<string, object?> artist) => new()
    {
        Id = artist.SafeGet<long>(ArtistColumns.Id),

        CreatedAt = artist.SafeGet<DateTime>(ArtistColumns.CreatedAt),
        UpdatedAt = artist.SafeGet<DateTime>(ArtistColumns.UpdatedAt),

        Alias = artist.SafeGet<string>(ArtistColumns.Alias),
        ArtworkUrl = artist.SafeGet<string>(ArtistColumns.ArtworkUrl),
        CalculatedGenres = artist.SafeGet<string>(ArtistColumns.CalculatedGenres),
        CalculatedInfluences = artist.SafeGet<string>(ArtistColumns.CalculatedInfluences),
        CurrentCountry = artist.SafeGet<string>(ArtistColumns.CurrentCountry),
        CurrentRegion = artist.SafeGet<string>(ArtistColumns.CurrentRegion),
        CurrentTown = artist.SafeGet<string>(ArtistColumns.CurrentTown),
        Description = artist.SafeGet<string>(ArtistColumns.Description),
        IsVisible = artist.SafeGet<bool>(ArtistColumns.IsVisible),
        Name = artist.SafeGet<string>(ArtistColumns.Name) ?? "",
        OriginCountry = artist.SafeGet<string>(ArtistColumns.OriginCountry),
        OriginRegion = artist.SafeGet<string>(ArtistColumns.OriginRegion),
        OriginTown = artist.SafeGet<string>(ArtistColumns.OriginTown),

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
        
        Alias = artist.Alias,
        ArtworkUrl = artist.ArtworkUrl,
        CurrentCountry = artist.CurrentCountry,
        CurrentRegion = artist.CurrentRegion,
        CurrentTown = artist.CurrentTown,
        Description = artist.Description,
        IsVisible = artist.IsVisible,
        Name = artist.Name,
        OriginCountry = artist.OriginCountry,
        OriginRegion = artist.OriginRegion,
        OriginTown = artist.OriginTown,

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