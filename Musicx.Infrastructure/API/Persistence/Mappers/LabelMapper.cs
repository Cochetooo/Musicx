using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Application.Shared.Helpers;
using Musicx.Infrastructure.API.Persistence.Columns;
using Newtonsoft.Json;

namespace Musicx.Infrastructure.API.Persistence.Mappers;

public static class LabelMapper
{
    private static readonly JsonSerializerSettings ReleaseMapperJsonOptions = new()
    {
        Converters =
        {
            new JsonToOutModelConverter<OutRelease>("release")
        }
    };

    public static OutLabel FromDicoToLabel(this IDictionary<string, object?> label) => new()
    {
        Id = label.SafeGet<long>(LabelColumns.Id),

        Releases = label.TryGetValue("releases", out var releaseValue)
                   && releaseValue is not null
            ? JsonConvert.DeserializeObject<OutRelease[]>(releaseValue as string ?? string.Empty,
                ReleaseMapperJsonOptions) ?? []
            : [],

        Description = label.SafeGet<string?>(LabelColumns.Description),
        IsVisible = label.SafeGet<bool>(LabelColumns.IsVisible),
        Name = label.SafeGet<string>(LabelColumns.Name) ?? string.Empty,
    };

    public static InLabel ToRaw(this OutLabel label) => new()
    {
        Id = label.Id,

        ReleaseIds = label.Releases.Select(r => r.Id).ToArray(),

        Description = label.Description,
        IsVisible = label.IsVisible,
        Name = label.Name,
    };
}