using Microsoft.AspNetCore.Components;
using MudBlazor;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Song;
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

    private MudDialog _modalRef = null!;

    protected override void OnAfterRender(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        _logger = LoggerProvider.CreateLogger(nameof(AlbumTrackListEditModal));
    }

    private async Task Save()
    {
        EnsureSongData();
        await UcSaveAll.ExecuteAsync(_songs);
        await OnSave.InvokeAsync();

        await Hide();
    }

    public async Task Show(OutAlbum album, List<OutSong>? songs = null)
    {
        _songs.Clear();
        
        if (null != songs)
        {
            foreach (var song in songs)
            {
                _songs.Add(song.ToRaw());
                StateHasChanged();
            }
        }
        else
        {
            AddRow();
        }
        
        _album = album;
        await _modalRef.ShowAsync();
    }

    private async Task Hide()
    {
        await _modalRef.CloseAsync();
    }

    private void DeleteRow(InSong? song)
    {
        if (song is null)
        {
            return;
        }
        
        _songs.Remove(song);
    }

    private void AddRow()
    {
        _songs.Add(new InSong());
    }

    private void EnsureSongData()
    {
        foreach (var song in _songs)
        {
            if (_album is not null)
            {
                song.AlbumId = _album.Id;
                song.ArtistId = _album.ArtistId;
            }
        }
    }
}