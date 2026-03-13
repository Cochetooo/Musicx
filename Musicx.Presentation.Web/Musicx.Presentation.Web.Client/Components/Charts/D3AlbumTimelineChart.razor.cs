using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Musicx.Presentation.Web.Client.Components.Charts;

public partial class D3AlbumTimelineChart : IAsyncDisposable
{
    [Parameter] public IReadOnlyList<AlbumTimelineEntry> Albums { get; set; } = [];
    [Parameter] public int Height { get; set; } = 360;
    [Parameter] public int YearTickInterval { get; set; } = 5;
    [Parameter] public EventCallback<long> OnAlbumClicked { get; set; }

    private ElementReference _containerRef;
    private bool _renderPending = true;
    private IJSObjectReference? _module;
    private DotNetObjectReference<D3AlbumTimelineChart>? _dotNetRef;

    protected override void OnParametersSet()
        => _renderPending = true;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!_renderPending)
        {
            return;
        }

        _module ??= await JS.InvokeAsync<IJSObjectReference>("import", "/Js/charts/albumTimelineChart.js");
        _dotNetRef ??= DotNetObjectReference.Create(this);
        _renderPending = false;

        await _module.InvokeVoidAsync("renderAlbumTimelineChart", _containerRef, new
        {
            height = Height,
            yearTickInterval = YearTickInterval,
            dotNetRef = _dotNetRef,
            callbackMethodName = nameof(NotifyAlbumClicked),
            albums = Albums.Select(album => new
            {
                id = album.Id,
                name = album.Name,
                releaseDate = album.OriginalReleaseDate,
                artworkUrl = album.ArtworkUrl,
                rating = album.AverageRating
            })
        });
    }

    [JSInvokable]
    public Task NotifyAlbumClicked(long albumId)
        => OnAlbumClicked.InvokeAsync(albumId);

    public async ValueTask DisposeAsync()
    {
        if (_module is not null)
        {
            await _module.InvokeVoidAsync("clearChart", _containerRef);
            await _module.DisposeAsync();
        }

        _dotNetRef?.Dispose();
    }
}

public sealed record AlbumTimelineEntry(long Id, string Name, DateTime OriginalReleaseDate, string ArtworkUrl, decimal AverageRating);