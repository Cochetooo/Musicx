using MudBlazor;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Song;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Genre;

namespace Musicx.Presentation.Web.Client.Modals.Admin.Albums;

public partial class AlbumTrackGenresEditModal
{
    private MudDialog _modalRef = null!;
    
    private List<OutGenre> _genres = [];
    
    private List<InSong> _albumSongs = [];
    private InSong? _selectedSong;

    private List<OutGenre> _selectedPrimaryGenres = [];
    private List<OutGenre> _selectedInfluenceGenres = [];

    private void AddPrimaryGenre(OutGenre genre)
        => _selectedPrimaryGenres.Add(genre);

    private void RemovePrimaryGenre(OutGenre genre)
        => _selectedPrimaryGenres.Remove(genre);
    
    private void AddInfluenceGenre(OutGenre genre)
        => _selectedInfluenceGenres.Add(genre);
    
    private void RemoveInfluenceGenre(OutGenre genre)
        => _selectedInfluenceGenres.Remove(genre);
}