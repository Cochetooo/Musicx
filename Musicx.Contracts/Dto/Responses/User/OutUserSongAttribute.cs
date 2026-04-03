namespace Musicx.Contracts.Dto.Responses.User;

public sealed class OutUserSongAttribute : BaseOutputModel
{
    public OutUser User { get; set; } = null!;
    public OutSong Song { get; set; } = null!;
    
    public short? Rating { get; set; }
    public short? ProductionRating { get; set; }
    public short? LyricsRating { get; set; }
    public short? InstrumentationRating { get; set; }
    public short? VocalsRating { get; set; }
    public short? AtmosphereRating { get; set; }
    public short? OriginalityRating { get; set; }
}