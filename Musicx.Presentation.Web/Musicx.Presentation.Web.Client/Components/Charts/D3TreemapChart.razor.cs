using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Musicx.Presentation.Web.Client.Components.Charts;

public partial class D3TreemapChart : IAsyncDisposable
{
    [Parameter] public object? Data { get; set; }
    [Parameter] public int Height { get; set; } = 500;

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

        _module ??= await JS.InvokeAsync<IJSObjectReference>("import", "/Js/charts/treemapChart.js");
        _renderPending = false;

        await _module.InvokeVoidAsync("renderTreemapChart", _containerRef, new { data = Data, height = Height });
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