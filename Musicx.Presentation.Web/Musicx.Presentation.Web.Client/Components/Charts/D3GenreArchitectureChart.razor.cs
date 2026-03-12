using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Musicx.Presentation.Web.Client.Components.Charts;

public partial class D3GenreArchitectureChart : IAsyncDisposable
{
    [Parameter] public IReadOnlyList<GenreArchitectureSeries> Data { get; set; } = [];
    [Parameter] public int Height { get; set; } = 520;
    [Parameter] public EventCallback<SubGenreClickEventArgs> OnSubGenreClicked { get; set; }

    private ElementReference _containerRef;
    private bool _renderPending = true;
    private IJSObjectReference? _module;
    private DotNetObjectReference<D3GenreArchitectureChart>? _dotNetRef;

    protected override void OnParametersSet() => _renderPending = true;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!_renderPending)
        {
            return;
        }

        _module ??= await JS.InvokeAsync<IJSObjectReference>("import", "/Js/charts/genreArchitectureChart.js");
        _dotNetRef ??= DotNetObjectReference.Create(this);
        _renderPending = false;

        await _module.InvokeVoidAsync("renderGenreArchitectureChart", _containerRef, new
        {
            height = Height,
            data = Data,
            dotNetRef = _dotNetRef,
            callbackMethodName = nameof(NotifySubGenreClicked)
        });
    }

    [JSInvokable]
    public Task NotifySubGenreClicked(string subGenreName, int year)
        => OnSubGenreClicked.InvokeAsync(new SubGenreClickEventArgs(subGenreName, year));

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

public sealed record GenreArchitectureSeries(string Genre, IReadOnlyList<GenreArchitectureEntry> Entries);
public sealed record GenreArchitectureEntry(int Year, double Value, IReadOnlyList<SubGenreEntry> SubGenres);
public sealed record SubGenreEntry(string Name, int Year, double Value);
public sealed record SubGenreClickEventArgs(string Name, int Year);