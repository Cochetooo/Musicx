using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Musicx.Presentation.Web.Client.Components.Charts;

public partial class D3LineChart
{
    [Parameter] public IReadOnlyList<D3LinePoint> Data { get; set; } = [];
    [Parameter] public int Height { get; set; } = 220;
    [Parameter] public int MarginTop { get; set; } = 12;
    [Parameter] public int MarginRight { get; set; } = 16;
    [Parameter] public int MarginBottom { get; set; } = 30;
    [Parameter] public int MarginLeft { get; set; } = 42;
    [Parameter] public string StrokeColor { get; set; } = "#7B61FF";
    [Parameter] public string FillColor { get; set; } = "rgba(123, 97, 255, 0.22)";
    [Parameter] public string GridColor { get; set; } = "rgba(255, 255, 255, 0.12)";
    [Parameter] public string TickColor { get; set; } = "rgba(255, 255, 255, 0.72)";
    
    private ElementReference _containerRef;
    private bool _renderPending = true;
    
    protected override void OnParametersSet()
    {
        _renderPending = true;
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!_renderPending)
        {
            return;
        }

        _renderPending = false;

        await JS.InvokeVoidAsync("d3Charts.renderLineChart", _containerRef, new
        {
            height = Height,
            marginTop = MarginTop,
            marginRight = MarginRight,
            marginBottom = MarginBottom,
            marginLeft = MarginLeft,
            strokeColor = StrokeColor,
            fillColor = FillColor,
            gridColor = GridColor,
            tickColor = TickColor,
            points = Data.Select(point => new
            {
                x = point.Label,
                y = point.Value
            })
        });
    }
    
    public async ValueTask DisposeAsync()
    {
        await JS.InvokeVoidAsync("d3Charts.clearChart", _containerRef);
    }
}

public sealed record D3LinePoint(string Label, double Value);