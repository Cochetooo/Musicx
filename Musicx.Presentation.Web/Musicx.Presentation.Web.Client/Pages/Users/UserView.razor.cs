using System.Resources;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Musicx.Application.Shared.Enums;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Contracts.Dto.Responses.Specifics.Ratings;
using Musicx.Infrastructure.API.Persistence.Specifications.User;

namespace Musicx.Presentation.Web.Client.Pages.Users;

public partial class UserView
{
    private ILogger _logger = null!;

    private MudTable<OutUserAlbumAttribute> _albumRatingsTable = null!;
    
    private OutUser? _user;

    private long _albumCount;
    private string _searchString = string.Empty;

    private OutUserRatingStats? _albumRatingDistrib;
    
    [Parameter] public string? Id { get; set; }
    
    protected override void OnInitialized()
    {
        _logger = LoggerFactory.CreateLogger(nameof(UserView));
    }

    protected override async Task OnParametersSetAsync()
    {
        await Load();
    }

    private async Task Load()
    {
        _albumCount = 0;
        _searchString = string.Empty;
        
        if (string.IsNullOrWhiteSpace(Id))
        {
            _logger.LogError("❌ Album ID is null or empty.");
            return;
        }
        
        if (!long.TryParse(Id, out var userId))
        {
            _logger.LogError("❌ Invalid User ID format: {Id}", Id);
            return;
        }

        _user = await UcGet.ExecuteAsync(userId, new UserJoinSpecification
        {
            IncludeRoles = true
        });

        if (_user == null)
        {
            _logger.LogError("❌ User has not been found.");
            return;
        }

        _logger.LogInformation($"✅ User loaded: {_user.Name} ({_user.Id})");
        await InvokeAsync(StateHasChanged);

        _albumRatingDistrib = await UcFindAlbumRatingDistrib.ExecuteAsync(_user.Id);
        _logger.LogInformation($"✅ Album Ratings Distribution loaded ({_user.Id})");
        
        await _albumRatingsTable.ReloadServerData();
        await InvokeAsync(StateHasChanged);
    }
    
    private async Task<TableData<OutUserAlbumAttribute>> LoadUserAttrData(TableState state, CancellationToken token)
    {
        if (_user is null)
        {
            _logger.LogWarning("⚠️ User ID is null, cannot load user attributes data.");
            return new TableData<OutUserAlbumAttribute>
            {
                TotalItems = 0,
                Items = []
            };
        }

        short sortDir = state.SortDirection == SortDirection.Ascending ? (short) 1 : (short) -1;
        var sortLabel = state.SortLabel;
        
        var response = await UcFindAlbumAttrs.ExecuteAsync(
            _user.Id, 
            pagingOptions: new PagingOptions(Take: state.PageSize, Skip: state.Page * state.PageSize),
            joins: new UserAlbumAttrJoinSpecification
            {
                IncludeAlbumArtists = true
            },
            order: new UserAlbumAttrOrderSpecification
            {
                AlbumOriginalReleaseDate = (sortLabel == "Album") ? (short) (sortDir * 2) : null,
                ArtistName = (sortLabel == "Album") ? sortDir : null,
                CollectionType = (sortLabel == "Collection") ? sortDir : null,
                Rating = (sortLabel == "Rating") ? sortDir : null,
                CreatedAt = (sortLabel == "Date") ? sortDir : null,
            },
            filter: string.IsNullOrWhiteSpace(_searchString) ? null : _searchString,
            cancellationToken: token
        );

        _albumCount = response.Total;
        await InvokeAsync(StateHasChanged);

        return new TableData<OutUserAlbumAttribute>
        {
            TotalItems = (int)response.Total,
            Items = response.Items
        };
    }

    private async Task OnClearRatingsButtonClicked()
    {
        if (_user is null)
        {
            _logger.LogWarning("⚠️ User ID is null, cannot try to clear ratings.");
            return;
        }
        
        var result = await DialogService.ShowMessageBox(
            "Warning",
            "Deleting all ratings cannot be undone! Are you sure you want to delete all your ratings?",
            yesText: "Delete!", cancelText: "Cancel");

        if (result is not null && result.Value)
        {
            await UcDeleteAllRatings.ExecuteAsync(_user.Id);
            await InvokeAsync(StateHasChanged);
        }

    }

    private void OnSearch(string text)
    {
        _searchString = text;
        _albumRatingsTable.ReloadServerData();
    }
}