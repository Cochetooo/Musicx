namespace Musicx.Contracts.Dto.Requests.User;

public sealed class InUserSongAttribute : BaseInputModel
{
    public long UserId { get; set; }
    public long SongId { get; set; }
    
    public short? Rating { get; set; }
    public short? ProductionRating { get; set; }
    public short? LyricsRating { get; set; }
    public short? InstrumentationRating { get; set; }
    public short? VocalsRating { get; set; }
    public short? AtmosphereRating { get; set; }
    public short? OriginalityRating { get; set; }
}