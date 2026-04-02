using Microsoft.JSInterop;
using Musicx.Application.Shared.Enums;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Genre;
using Musicx.Infrastructure.API.Persistence.Specifications.Genre;

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
        _genres = (await UcList.ExecuteAsync(
            pagingOptions: new PagingOptions(Take: 100_000, Skip: 0),
            joins: new GenreJoinSpecification
            {
                IncludeChildren = true,
                IncludeParents = true
            })).Items;
        
        _logger.LogInformation("✅ Genres retrieved successfully!");
        await InvokeAsync(StateHasChanged);
    }
}