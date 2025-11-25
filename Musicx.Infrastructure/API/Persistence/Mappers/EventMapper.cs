using Musicx.Contracts.Dto.Jsons.Events;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Helpers;
using Musicx.Infrastructure.API.Persistence.Columns;
using Newtonsoft.Json;
using JsonConverter = System.Text.Json.Serialization.JsonConverter;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Musicx.Infrastructure.API.Persistence.Mappers;

public static class EventMapper
{
    private static readonly JsonSerializerSettings EventArtistMapperJsonOptions = new()
    {
        Converters =
        {
            new JsonToOutModelConverter<OutEventArtist>("event_artist")
        }
    };
    
    private static readonly JsonSerializerSettings EventUserMapperJsonOptions = new()
    {
        Converters =
        {
            new JsonToOutModelConverter<OutEventUser>("event_user")
        }
    };

    public static OutEvent FromDicoToEvent(this IDictionary<string, object?> _event) => new()
    {
        Id = _event.SafeGet<long>(EventColumns.Id),

        EventArtists = _event.TryGetValue("event_artists", out var eventArtistValue)
                       && eventArtistValue is not null
            ? JsonConvert.DeserializeObject<OutEventArtist[]>(eventArtistValue as string ?? string.Empty,
                EventArtistMapperJsonOptions)
            : null,

        EventUsers = _event.TryGetValue("event_users", out var eventUserValue)
                     && eventUserValue is not null
            ? JsonConvert.DeserializeObject<OutEventUser[]>(eventUserValue as string ?? string.Empty,
                EventUserMapperJsonOptions)
            : null,
        
        EventPrices = _event.TryGetValue("event_prices", out var eventPricesValue)
                      && eventPricesValue is not null
            ? JsonConvert.DeserializeObject<EventPrice[]>(eventPricesValue as string ?? string.Empty)
            : null,
        
        EventTicketLinks = _event.TryGetValue("event_ticket_links", out var eventTicketLinksValue)
                      && eventTicketLinksValue is not null
            ? JsonConvert.DeserializeObject<EventTicketLink[]>(eventTicketLinksValue as string ?? string.Empty)
            : null,
        
        Address = _event.SafeGet<string?>(EventColumns.Address),
        BeginDate = _event.SafeGet<DateTime?>(EventColumns.BeginDate),
        Country = _event.SafeGet<string?>(EventColumns.Country),
        Description = _event.SafeGet<string?>(EventColumns.Description),
        EndDate = _event.SafeGet<DateTime?>(EventColumns.EndDate),
        IsFestival = _event.SafeGet<bool>(EventColumns.IsFestival),
        IsVisible = _event.SafeGet<bool>(EventColumns.IsVisible),
        Name = _event.SafeGet<string>(EventColumns.Name) ?? string.Empty,
        PosterUrl = _event.SafeGet<string?>(EventColumns.PosterUrl),
        Town = _event.SafeGet<string?>(EventColumns.Town),
        Venue = _event.SafeGet<string?>(EventColumns.Venue),
        ZipCode = _event.SafeGet<string?>(EventColumns.ZipCode),
    };

    public static InEvent ToRaw(this OutEvent _event) => new()
    {
        Id = _event.Id,
        
        EventPrices = JsonSerializer.Serialize(_event.EventPrices),
        EventTicketLinks = JsonSerializer.Serialize(_event.EventTicketLinks),
        
        Address = _event.Address,
        BeginDate = _event.BeginDate,
        Country = _event.Country,
        Description = _event.Description,
        EndDate = _event.EndDate,
        IsFestival = _event.IsFestival,
        IsVisible = _event.IsVisible,
        Name = _event.Name,
        PosterUrl = _event.PosterUrl,
        Town = _event.Town,
        Venue = _event.Venue,
        ZipCode = _event.ZipCode,
    };
}