using Musicx.Contracts.Dto.Responses.Artist;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Contracts.Enums;

namespace Musicx.Contracts.Dto.Responses.Specifics.Artists;

public sealed class OutArtistDataView
{
    public OutArtist Artist { get; set; } = null!;
    public OutAlbumList Albums { get; set; } = new();
    public IReadOnlyList<OutArtistGenreStat> PrimaryGenres { get; set; } = [];
    public IReadOnlyList<OutArtistGenreStat> Influences { get; set; } = [];
    public IReadOnlyList<OutArtistGenreStat> Descriptors { get; set; } = [];
    public IReadOnlyList<OutArtistGenreStat> Scenes { get; set; } = [];
    public IReadOnlyList<OutArtistGenreStat> Movements { get; set; } = [];
    public bool IsCurrentUserFollowing { get; set; }
    public long FollowersCount { get; set; }
    public OutGenericList<OutUserAlbumAttribute>? UserAttributes { get; set; }
}