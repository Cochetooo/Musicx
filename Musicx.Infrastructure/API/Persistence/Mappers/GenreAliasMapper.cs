using System.Text.Json.Nodes;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Helpers;
using Musicx.Infrastructure.API.Persistence.Columns;

namespace Musicx.Infrastructure.API.Persistence.Mappers;

public static class GenreAliasMapper
{
    public static OutGenreAlias FromDicoToGenreAlias(this IDictionary<string, object?> genreAlias) => new()
    {
        Id = genreAlias.SafeGet<long>(GenreAliasColumns.Id),

        Genre = genreAlias.FromDicoToGenre(),

        Lang = genreAlias.SafeGet<string?>(GenreAliasColumns.Lang),
        Metadata = genreAlias.SafeGet<JsonObject?>(GenreAliasColumns.Metadata),
        Name = genreAlias.SafeGet<string>(GenreAliasColumns.Name) ?? string.Empty,
    };

    public static InGenreAlias ToRaw(this OutGenreAlias genreAlias) => new()
    {
        Id = genreAlias.Id,
        GenreId = genreAlias.Genre.Id,
        
        Lang = genreAlias.Lang,
        Metadata = genreAlias.Metadata?.ToJsonString(),
        Name = genreAlias.Name,
    };
}