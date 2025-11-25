namespace Musicx.Contracts.Dto.Requests;

public sealed class InUserSongAttribute : BaseInputModel
{
    public long UserId { get; set; }
    public long SongId { get; set; }
    
    public short? Rating { get; set; }
}