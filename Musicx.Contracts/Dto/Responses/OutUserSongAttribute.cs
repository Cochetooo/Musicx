namespace Musicx.Contracts.Dto.Responses;

public sealed class OutUserSongAttribute : BaseOutputModel
{
    public OutUser User { get; set; } = null!;
    public OutSong Song { get; set; } = null!;
    
    public short? Rating { get; set; }
}