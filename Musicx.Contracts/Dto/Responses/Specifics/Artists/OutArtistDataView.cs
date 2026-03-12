using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Contracts.Enums;

namespace Musicx.Contracts.Dto.Responses.Specifics.Artists;

public sealed class OutArtistDataView
{
    public OutArtist Artist { get; set; } = null!;
    public OutAlbumList Albums { get; set; } = new();
    public OutArtistRatingSummary RatingSummary { get; set; } = new();
    public OutGenericList<OutUserAlbumAttribute>? UserAttributes { get; set; }
}