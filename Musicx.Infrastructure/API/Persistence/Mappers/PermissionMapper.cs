using System.Security;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Application.Shared.Helpers;
using Musicx.Contracts.Dto.Requests.Security;
using Musicx.Infrastructure.API.Persistence.Columns;
using Musicx.Infrastructure.API.Persistence.Columns.Security;

namespace Musicx.Infrastructure.API.Persistence.Mappers;

public static class PermissionMapper
{
    public static OutPermission FromDicoToPermission(this IDictionary<string, object?> permission) => new()
    {
        Id = permission.SafeGet<long>(PermissionColumns.Id),
        
        CreatedAt = permission.SafeGet<DateTime>(PermissionColumns.CreatedAt),
        UpdatedAt = permission.SafeGet<DateTime>(PermissionColumns.UpdatedAt),
        
        Name = permission.SafeGet<string>(PermissionColumns.Name) ?? "",
    };

    public static InPermission ToRaw(this OutPermission permission) => new()
    {
        Id = permission.Id,

        Name = permission.Name
    };
}