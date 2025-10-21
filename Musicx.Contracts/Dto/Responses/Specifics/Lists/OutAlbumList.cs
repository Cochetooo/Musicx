namespace Musicx.Contracts.Dto.Responses.Specifics.Lists;

public sealed class OutAlbumList : OutGenericList<OutAlbum>
{
    public decimal? AverageRating { get; set; }
}