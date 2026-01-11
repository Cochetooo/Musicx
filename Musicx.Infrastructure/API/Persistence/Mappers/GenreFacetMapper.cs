using System.Text.Json.Nodes;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Application.Shared.Helpers;
using Musicx.Contracts.Dto.Requests.Genre;
using Musicx.Infrastructure.API.Persistence.Columns;
using Musicx.Infrastructure.API.Persistence.Columns.Genre;

namespace Musicx.Infrastructure.API.Persistence.Mappers;

public static class GenreFacetMapper
{
    public static OutGenreFacet FromDicoToGenreFacet(this IDictionary<string, object?> genreFacet) => new()
    {
        Facet = genreFacet.FromDicoToFacet(),
        Genre = genreFacet.FromDicoToGenre(),
        
        Confidence = genreFacet.SafeGet<float>(GenreFacetColumns.Confidence),
        Metadata = genreFacet.SafeGet<string?>(GenreFacetColumns.Metadata),
        Value = genreFacet.SafeGet<string>(GenreFacetColumns.Value) ?? string.Empty,
    };

    public static InGenreFacet ToRaw(this OutGenreFacet genreFacet) => new()
    {
        FacetId = genreFacet.Facet.Id,
        GenreId = genreFacet.Genre.Id,
        
        Confidence = genreFacet.Confidence,
        Metadata = genreFacet.Metadata,
        Value = genreFacet.Value,
    };
}