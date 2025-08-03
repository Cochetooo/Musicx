using Musicx.Contracts.Dto.Responses;

namespace Musicx.Presentation.Web.Client.Pages;

public partial class GenreList
{
    private ILogger _logger = null!;
    
    private List<OutGenre> _genres = [];

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        _logger = LoggerProvider.CreateLogger(nameof(GenreList));

        await Load();
    }

    private async Task Load()
    {
        _genres = await UcList.ExecuteAsync(take: 10_000, query: "parents_children");
        
        _logger.LogInformation("✅ Genres retrieved successfully!");
        await InvokeAsync(StateHasChanged);
    }
}