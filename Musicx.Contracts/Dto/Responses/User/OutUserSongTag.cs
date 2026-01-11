namespace Musicx.Contracts.Dto.Responses;

public sealed class OutUserSongTag : BaseOutputModel
{
    public OutUser User { get; set; } = null!;
    public OutSong Song { get; set; } = null!;
    public OutTag Tag { get; set; } = null!;
}