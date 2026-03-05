using Musicx.Application.Api.Models.Auth;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Enums;
using Musicx.Application.Shared.Helpers;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Contracts.Dto.Responses.User;
using Musicx.Infrastructure.API.Persistence.Columns;
using Musicx.Infrastructure.API.Persistence.Columns.User;
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

        BannerUrl = user.SafeGet<string?>(UserColumns.BannerUrl),
        Biography = user.SafeGet<string?>(UserColumns.Biography),
        BirthDate = user.SafeGet<DateTime?>(UserColumns.BirthDate),
        Email = user.SafeGet<string>(UserColumns.Email) ?? "",
        EmailConfirmed = user.SafeGet<bool>(UserColumns.EmailConfirmed),
        Name = user.SafeGet<string>(UserColumns.Name) ?? "",
        GoogleId = user.SafeGet<string?>(UserColumns.GoogleId),
        LastFmUsername = user.SafeGet<string?>(UserColumns.LastFmUsername),
        PrefBannerBlur = user.SafeGet<bool>(UserColumns.PrefBannerBlur),
        PrefDarkMode = user.SafeGet<bool>(UserColumns.PrefDarkMode),
        PrefRatingMode = user.SafeGet<RatingMode>(UserColumns.PrefRatingMode),
        PrefShowRatings = user.SafeGet<bool>(UserColumns.PrefShowRatings),
        PrefSimpleGenre = user.SafeGet<bool>(UserColumns.PrefSimpleGenre),
        PictureUrl = user.SafeGet<string?>(UserColumns.PictureUrl),
    };
    
    public static OutUserAuth FromDicoToUserAuth(this IDictionary<string, object?> user) => new()
    {
        User = user.FromDicoToUser(),
        PasswordHash = user.SafeGet<string?>(UserColumns.PasswordHash),
        PasswordSalt = user.SafeGet<string?>(UserColumns.PasswordSalt),
    };

    public static InUser ToRaw(this OutUser user) => new()
    {
        Id = user.Id,

        RoleIds = user.Roles?.Select(r => r.Id).ToList(),

        BannerUrl = user.BannerUrl,
        Biography = user.Biography,
        BirthDate = user.BirthDate,
        Email = user.Email,
        EmailConfirmed = user.EmailConfirmed,
        Name = user.Name,
        GoogleId = user.GoogleId,
        LastFmUsername = user.LastFmUsername,
        PrefBannerBlur = user.PrefBannerBlur,
        PrefDarkMode = user.PrefDarkMode,
        PrefRatingMode = user.PrefRatingMode,
        PrefShowRatings = user.PrefShowRatings,
        PrefSimpleGenre = user.PrefSimpleGenre,
        PictureUrl = user.PictureUrl,
    };
}