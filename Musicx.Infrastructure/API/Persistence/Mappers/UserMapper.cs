using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Helpers;
using Musicx.Infrastructure.API.Persistence.Columns;
using Newtonsoft.Json;

namespace Musicx.Infrastructure.API.Persistence.Mappers;

public static class UserMapper
{
    private static readonly JsonSerializerSettings RoleMapperJsonOptions = new()
    {
        Converters =
        {
            new JsonToOutModelConverter<OutRole>("role")
        }
    };

    public static OutUser FromDicoToUser(this IDictionary<string, object?> user) => new()
    {
        Id = user.SafeGet<long>(UserColumns.Id),

        CreatedAt = user.SafeGet<DateTime>(UserColumns.CreatedAt),
        UpdatedAt = user.SafeGet<DateTime>(UserColumns.UpdatedAt),
        
        Roles = user.TryGetValue("roles", out var roleValue)
                && roleValue is not null
            ? JsonConvert.DeserializeObject<OutRole[]>(roleValue as string ?? string.Empty,
                RoleMapperJsonOptions)
            : null,

        Email = user.SafeGet<string>(UserColumns.Email) ?? "",
        EmailConfirmed = user.SafeGet<bool>(UserColumns.EmailConfirmed),
        Name = user.SafeGet<string>(UserColumns.Name) ?? "",
        PasswordHash = user.SafeGet<string>(UserColumns.PasswordHash),
        PasswordSalt = user.SafeGet<string>(UserColumns.PasswordSalt),
        GoogleId = user.SafeGet<string>(UserColumns.GoogleId),
        LastFmUsername = user.SafeGet<string>(UserColumns.LastFmUsername),
    };

    public static InUser ToRaw(this OutUser user) => new()
    {
        Id = user.Id,

        RoleIds = user.Roles?.Select(r => r.Id).ToList(),

        Email = user.Email,
        EmailConfirmed = user.EmailConfirmed,
        Name = user.Name,
        PasswordHash = user.PasswordHash,
        PasswordSalt = user.PasswordSalt,
        GoogleId = user.GoogleId,
        LastFmUsername = user.LastFmUsername
    };
}