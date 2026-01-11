using System.Text.Json.Nodes;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Application.Shared.Helpers;
using Musicx.Contracts.Dto.Requests.Genre;
using Musicx.Infrastructure.API.Persistence.Columns;
using Musicx.Infrastructure.API.Persistence.Columns.Genre;

namespace Musicx.Infrastructure.API.Persistence.Mappers;

public static class GenreAliasMapper
{
    public static OutGenreAlias FromDicoToGenreAlias(this IDictionary<string, object?> genreAlias) => new()
    {
        Id = genreAlias.SafeGet<long>(GenreAliasColumns.Id),

        Genre = genreAlias.FromDicoToGenre(),

        Lang = genreAlias.SafeGet<string?>(GenreAliasColumns.Lang),
        Metadata = genreAlias.SafeGet<string?>(GenreAliasColumns.Metadata),
        Name = genreAlias.SafeGet<string>(GenreAliasColumns.Name) ?? string.Empty,
    };

    public static InGenreAlias ToRaw(this OutGenreAlias genreAlias) => new()
    {
        Id = genreAlias.Id,
        GenreId = genreAlias.Genre.Id,
        
        Lang = genreAlias.Lang,
        Metadata = genreAlias.Metadata,
        Name = genreAlias.Name,
    };
}