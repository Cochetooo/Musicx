using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Application.Shared.Helpers;
using Musicx.Contracts.Dto.Requests.Event;
using Musicx.Infrastructure.API.Persistence.Columns;
using Musicx.Infrastructure.API.Persistence.Columns.Event;

namespace Musicx.Infrastructure.API.Persistence.Mappers;

public static class EventUserMapper
{
    public static OutEventUser FromDicoToEventUser(this IDictionary<string, object?> eventUser) => new()
    {
        Event = eventUser.FromDicoToEvent(),
        User = eventUser.FromDicoToUser(),

        Comment = eventUser.SafeGet<string?>(EventUserColumns.Comment),
        IsGoing = eventUser.SafeGet<bool>(EventUserColumns.IsGoing),
    };

    public static InEventUser ToRaw(this OutEventUser eventUser) => new()
    {
        EventId = eventUser.Event.Id,
        UserId = eventUser.User.Id,
        
        Comment = eventUser.Comment,
        IsGoing = eventUser.IsGoing,
    };
}