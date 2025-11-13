using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Helpers;
using Musicx.Infrastructure.API.Persistence.Columns;

namespace Musicx.Infrastructure.API.Persistence.Mappers;

public static class TagMapper
{
    public static OutTag FromDicoToTag(this IDictionary<string, object?> tag) => new()
    {
        Id = tag.SafeGet<long>(TagColumns.Id),
        
        CreatedAt = tag.SafeGet<DateTime>(TagColumns.CreatedAt),
        UpdatedAt = tag.SafeGet<DateTime>(TagColumns.UpdatedAt),
        
        Name = tag.SafeGet<string>(TagColumns.Name) ?? "",
    };

    public static InTag ToRaw(this OutTag tag) => new()
    {
        Id = tag.Id,

        Name = tag.Name,
    };
}