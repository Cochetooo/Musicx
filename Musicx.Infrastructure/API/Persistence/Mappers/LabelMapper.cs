using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Helpers;
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
}