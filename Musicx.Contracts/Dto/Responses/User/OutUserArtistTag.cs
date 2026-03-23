using Musicx.Contracts.Dto.Responses.Artist;
using Musicx.Contracts.Dto.Responses.User;

namespace Musicx.Contracts.Dto.Responses;

public sealed class OutUserArtistTag : BaseOutputModel
{
    public OutUser User { get; set; } = null!;
    public OutArtist Artist { get; set; } = null!;
    public OutTag Tag { get; set; } = null!;
}