namespace Musicx.Contracts.Dto.Requests;

public sealed class InUserSongRating : BaseInputModel
{
    public long UserId { get; set; }
    public long SongId { get; set; }
    
    public short Rating { get; set; }
}