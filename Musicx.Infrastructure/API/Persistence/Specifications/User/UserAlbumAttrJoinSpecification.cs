using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.User;

namespace Musicx.Infrastructure.API.Persistence.Specifications.User;

/// <summary>
/// Specify the relation to include when retrieving user album attribute from repository.
/// </summary>
/// <since>0.7.0</since>
public sealed class UserAlbumAttrJoinSpecification : IJoinSpecification<InUserAlbumAttribute>
{
    /// <summary>
    /// Include the artist of the album of this user-album-attribute.
    /// </summary>
    /// <since>0.7.0/since>
    public bool IncludeAlbumArtists { get; set; }
}