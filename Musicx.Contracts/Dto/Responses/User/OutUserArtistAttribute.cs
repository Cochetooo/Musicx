namespace Musicx.Contracts.Dto.Responses;

public sealed class OutUserArtistAttribute : BaseOutputModel
{
    public OutUser User { get; set; } = null!;
    public OutArtist Artist { get; set; } = null!;
    
    public bool? Follow { get; set; }
    public short? Rating { get; set; }
}