using Musicx.Contracts.Dto.Requests.Specifics;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;

namespace Musicx.Presentation.Web.Client.Pages;

public partial class Charts
{
    private OutAlbumList _outAlbumList = new();
    private AlbumChartQuery _query = new();

    private bool _isLoading = false;

    protected override async Task OnInitializedAsync()
    {
        await LoadChart();
    }

    private async Task LoadChart()
    {
        _isLoading = true;
        await InvokeAsync(StateHasChanged);
        
        _outAlbumList = await UcAlbumChart.ExecuteAsync(_query);
        
        _isLoading = false;
        await InvokeAsync(StateHasChanged);
    }
}