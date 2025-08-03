using Blazorise;
using Microsoft.AspNetCore.Components;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Infrastructure.API.Persistence.Mappers;

namespace Musicx.Presentation.Web.Client.Modals.Admin.Albums;

public partial class AlbumTrackListEditModal
{
    private ILogger _logger = null!;
    
    [Parameter] public EventCallback OnSave { get; set; }
    [Parameter] public EventCallback OnClose { get; set; }

    private OutAlbum? _album;
    private readonly List<InSong> _songs = [];

    private Modal _modalRef = null!;

    protected override void OnAfterRender(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        _logger = LoggerProvider.CreateLogger(nameof(AlbumTrackListEditModal));
    }

    public void Show(OutAlbum album, List<OutSong>? songs = null)
    {
        if (null != songs)
        {
            _songs.Clear();
            foreach (var song in songs)
            {
                _songs.Add(song.ToRaw());
                StateHasChanged();
            }
        }
        
        _album = album;
        EnsureSongData();
        _modalRef.Show();
    }

    private void Hide()
    {
        _modalRef.Hide();
    }

    private void DeleteRow(InSong song)
    {
        _songs.Remove(song);
    }

    private void AddRow()
    {
        _songs.Add(new InSong());
        EnsureSongData();
    }

    private void EnsureSongData()
    {
        foreach (var song in _songs)
        {
            if (_album is not null)
            {
                song.AlbumId = _album.Id;

                if (_album.Artist is not null)
                {
                    song.ArtistId = _album.Artist.Id;
                }
            }
        }
    }
}