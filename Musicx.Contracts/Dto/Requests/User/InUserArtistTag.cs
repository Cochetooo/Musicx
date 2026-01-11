namespace Musicx.Contracts.Dto.Requests.User;

public sealed class InUserArtistTag
{
    public long UserId { get; set; }
    public long ArtistId { get; set; }
    public long TagId { get; set; }
}