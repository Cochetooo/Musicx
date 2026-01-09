using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Application.Shared.Helpers;
using Musicx.Infrastructure.API.Persistence.Columns;

namespace Musicx.Infrastructure.API.Persistence.Mappers;

public static class EventArtistMapper
{
    public static OutEventArtist FromDicoToEventUser(this IDictionary<string, object?> eventArtist) => new()
    {
        Event = eventArtist.FromDicoToEvent(),
        Artist = eventArtist.FromDicoToArtist(),

        BeginDate = eventArtist.SafeGet<DateTime?>(EventArtistColumns.BeginDate),
        EndDate = eventArtist.SafeGet<DateTime>(EventArtistColumns.EndDate),
    };

    public static InEventArtist ToRaw(this OutEventArtist eventArtist) => new()
    {
        EventId = eventArtist.Event.Id,
        ArtistId = eventArtist.Artist.Id,
        
        BeginDate = eventArtist.BeginDate,
        EndDate = eventArtist.EndDate,
    };
}