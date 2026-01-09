using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Enums;
using Musicx.Application.Shared.Helpers;
using Musicx.Infrastructure.API.Persistence.Columns;

namespace Musicx.Infrastructure.API.Persistence.Mappers;

public static class FacetMapper
{
    public static OutFacet FromDicoToFacet(this IDictionary<string, object?> facet) => new()
    {
        Id = facet.SafeGet<long>(FacetColumns.Id),
        
        Description = facet.SafeGet<string?>(FacetColumns.Description),
        Name = facet.SafeGet<string?>(FacetColumns.Name) ?? string.Empty,
        Type = facet.SafeGet<FacetType>(FacetColumns.Type),
    };

    public static InFacet ToRaw(this OutFacet facet) => new()
    {
        Id = facet.Id,
        
        Description = facet.Description,
        Name = facet.Name,
        Type = facet.Type,
    };
}