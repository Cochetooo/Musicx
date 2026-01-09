using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Application.Shared.Helpers;
using Musicx.Infrastructure.API.Persistence.Columns;
using Newtonsoft.Json;

namespace Musicx.Infrastructure.API.Persistence.Mappers;

public static class RoleMapper
{
    private static readonly JsonSerializerSettings RoleMapperJsonOptions = new()
    {
        Converters =
        {
            new JsonToOutModelConverter<OutPermission>("permission")
        }
    };
    
    public static OutRole FromDicoToRole(this IDictionary<string, object?> role) => new()
    {
        Id = role.SafeGet<long>(RoleColumns.Id),
        
        CreatedAt = role.SafeGet<DateTime>(RoleColumns.CreatedAt),
        UpdatedAt = role.SafeGet<DateTime>(RoleColumns.UpdatedAt),
        
        Permissions = role.TryGetValue("permissions", out var permissionValue)
                && permissionValue is not null
            ? JsonConvert.DeserializeObject<OutPermission[]>(permissionValue as string ?? string.Empty,
                RoleMapperJsonOptions)
            : null,
        
        Name = role.SafeGet<string>(RoleColumns.Name) ?? "",
    };
    
    public static InRole ToRaw(this OutRole role) => new()
    {
        Id = role.Id,

        PermissionIds = role.Permissions?.Select(p => p.Id).ToList(),
        
        Name = role.Name
    };
}