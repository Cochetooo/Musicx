using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Musicx.Presentation.Web.Client.Components.Charts;

public partial class D3BrushableScatterMatrix : IAsyncDisposable
{
    [Parameter] public IReadOnlyList<object> Data { get; set; } = [];
    [Parameter] public IReadOnlyList<string> Dimensions { get; set; } = [];
    [Parameter] public int Size { get; set; } = 130;

    private ElementReference _containerRef;
    private bool _renderPending = true;
    private IJSObjectReference? _module;

    protected override void OnParametersSet() => _renderPending = true;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!_renderPending)
        {
            return;
        }

        _module ??= await JS.InvokeAsync<IJSObjectReference>("import", "/Js/charts/brushableScatterMatrix.js");
        _renderPending = false;

        await _module.InvokeVoidAsync("renderBrushableScatterMatrix", _containerRef, new
        {
            data = Data,
            dimensions = Dimensions,
            size = Size
        });
    }

    public async ValueTask DisposeAsync()
    {
        if (_module is not null)
        {
            await _module.InvokeVoidAsync("clearChart", _containerRef);
            await _module.DisposeAsync();
        }
    }
}