using Musicx.Contracts.Dto.Requests.User;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.User;

namespace Musicx.Application.Web.ViewModels.Album;

public sealed class AlbumViewModel
{
    public OutAlbum? Album { get; set; }
    public List<OutSong> Songs { get; set; } = [];
    public OutAlbum? PreviousAlbum { get; set; }
    public OutAlbum? NextAlbum { get; set; }

    public InUserAlbumAttribute UserAttribute { get; set; } = new();

    public IReadOnlyList<OutUserSongAttribute> UserSongAttributes { get; set; } = [];

    public List<OutAlbum> SimilarAlbums { get; set; } = [];
    
    public List<OutUserAlbumAttribute> Reviews { get; set; } = [];
    public long ReviewsTotal { get; set; }
    public int ReviewsPage { get; set; } = 1;

    public Dictionary<string, short> FactorAverages { get; set; } = [];

    public bool ShowAdvancedFactors { get; set; }
    public bool ShowReviewEditor { get; set; }
    public bool AutoComputeRating { get; set; }
    public bool ShowDetailedGenres { get; set; }
    public bool IsArtworkRevealed { get; set; }
}