namespace Musicx.Contracts.Dto.Responses;

public sealed class OutUserSongRating : BaseOutputModel
{
    public OutUser User { get; set; } = null!;
    public OutSong Song { get; set; } = null!;
    
    public long UserId { get; set; }
    public long SongId { get; set; }
    
    public short Rating { get; set; }
}