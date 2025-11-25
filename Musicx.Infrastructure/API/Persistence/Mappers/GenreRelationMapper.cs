using System.Text.Json.Nodes;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Enums;
using Musicx.Contracts.Helpers;
using Musicx.Infrastructure.API.Persistence.Columns;

namespace Musicx.Infrastructure.API.Persistence.Mappers;

public static class GenreRelationMapper
{
    public static OutGenreRelation FromDicoToGenreRelation(this IDictionary<string, object?> genreRelation) => new()
    {
        FromGenre = genreRelation.FromDicoToGenre("fg."),
        ToGenre = genreRelation.FromDicoToGenre("tg."),

        Metadata = genreRelation.SafeGet<string?>(GenreRelationColumns.Metadata),
        Type = genreRelation.SafeGet<GenreRelationType>(GenreRelationColumns.Type),
        Weight = genreRelation.SafeGet<float>(GenreRelationColumns.Weight),
    };

    public static InGenreRelation ToRaw(this OutGenreRelation genreRelation) => new()
    {
        FromGenreId = genreRelation.FromGenre.Id,
        ToGenreId = genreRelation.ToGenre.Id,

        Metadata = genreRelation.Metadata,
        Type = genreRelation.Type,
        Weight = genreRelation.Weight,
    };
}