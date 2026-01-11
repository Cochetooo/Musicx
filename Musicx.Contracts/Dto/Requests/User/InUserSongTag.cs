namespace Musicx.Contracts.Dto.Requests.User;

public sealed class InUserSongTag
{
    public long UserId { get; set; }
    public long SongId { get; set; }
    public long TagId { get; set; }
}