using System.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Helpers;
using Musicx.Application.Web.Interfaces.Models.User.Ratings;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Genre;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Contracts.Dto.Responses.Specifics.Ratings;
using Musicx.Contracts.Dto.Responses.User;
using Musicx.Contracts.Enums;
using Musicx.Infrastructure.API.Persistence.Specifications.User;

namespace Musicx.Presentation.Web.Client.Pages.Users;

public partial class UserView : IAsyncDisposable
{
    private ILogger _logger = null!;

    private MudTable<OutUserAlbumAttribute> _albumRatingsTable = null!;
    private MudDialog _exportRatingsModal = null!;
    
    private OutUser? _user;

    private long _albumCount;
    private string _searchString = string.Empty;

    private List<OutAlbum> _favAlbums = [];

    private OutUserRatingStats? _albumRatingDistrib;
    private int _maxRatingDistribCount;
    private UserRatingsExportFormat _selectedExportFormat = UserRatingsExportFormat.Csv;
    
    private IJSObjectReference? _fileDownloadModule;

    private int? UserAge
    {
        get
        {
            if (_user?.BirthDate is null)
            {
                return null;
            }

            var today = DateTime.Today;
            var birth = _user.BirthDate.Value.Date;
            
            var age = today.Year - birth.Year;

            if (birth.Date > today.AddYears(-age))
            {
                age--;
            }

            return age >= 0 ? age : null;
        }
    }
    
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
        GenreRatingsViewModel.Reset();
        
        _albumCount = 0;
        _searchString = string.Empty;
        _favAlbums.Clear();
        
        if (!TryGetUserId(out var userId))
        {
            Snackbar.Add(T["Web.UserView.InvalidUser"], Severity.Warning);
            return;
        }

        var dataView = await UcUserDataView.ExecuteAsync(userId, UserClientContext.CurrentUser?.Id);
        if (dataView is null)
        {
            _logger.LogError("❌ User DataView has not been found.");
            Snackbar.Add(T["Web.UserView.UserNotFound"], Severity.Warning);
            return;
        }

        _user = dataView.User;
        _albumRatingDistrib = dataView.RatingStats;
        _maxRatingDistribCount = _albumRatingDistrib?.RatingCounts.Values.Max() ?? 1;
        _favAlbums = dataView.FavoriteAlbums.ToList();

        if (dataView.TopGenres.Count > 0)
        {
            await GenreRatingsViewModel.LoadAsync(UcFindUserGenreRatings, _user.Id, 5);
        }

        _logger.LogInformation("✅ User loaded from DataView: {Name} ({Id})", _user.Name, _user.Id);
        
        await InvokeAsync(StateHasChanged);
        await _albumRatingsTable.ReloadServerData();
        await InvokeAsync(StateHasChanged);
    }
    
    private bool TryGetUserId(out long userId)
    {
        userId = 0;

        if (string.IsNullOrWhiteSpace(Id))
        {
            _logger.LogError("❌ User ID is null or empty.");
            return false;
        }

        if (!long.TryParse(Id, out userId))
        {
            _logger.LogError("❌ Invalid User ID format: {Id}", Id);
            return false;
        }

        return true;
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
            return;
        }
        
        var result = await DialogService.ShowMessageBoxAsync(
            T["Web.Common.Warning"],
            T["Web.UserView.DeleteRatingsWarning"],
            yesText: T["Web.Common.Delete"], cancelText: T["Web.Common.Cancel"]);

        if (result is not null && result.Value)
        {
            await UcDeleteAllRatings.ExecuteAsync(_user.Id);
            Snackbar.Add(T["Web.Common.Success"], Severity.Success);
            await Load();
        }

    }

    private async Task OnExportRatingsButtonClicked()
    {
        await _exportRatingsModal.ShowAsync();
    }

    private async Task OnCancelExportButtonClicked()
    {
        await _exportRatingsModal.CloseAsync();
    }

    private async Task OnConfirmExportButtonClicked()
    {
        if (_user is null)
        {
            Snackbar.Add(T["Web.UserView.UserNotFoundExport"], Severity.Warning);
            await _exportRatingsModal.CloseAsync();
            return;
        }

        var ratings = await LoadAllUserRatings();
        if (ratings.Count == 0)
        {
            Snackbar.Add(T["Web.UserView.NoRatingsToExport"], Severity.Info);
            await _exportRatingsModal.CloseAsync();
            return;
        }

        var exportFile = UcExportRatings.Execute(ratings, _user.Name, _selectedExportFormat);
        _fileDownloadModule ??= await JS.InvokeAsync<IJSObjectReference>("import", "/Js/fileDownload.js");
        await _fileDownloadModule.InvokeVoidAsync(
            "downloadFileFromBytes",
            exportFile.FileName,
            exportFile.ContentType,
            Convert.ToBase64String(exportFile.Content));

        Snackbar.Add(T["Web.UserView.RatingsExportSuccess"], Severity.Success);
        await _exportRatingsModal.CloseAsync();
    }

    private async Task OnExpandTopGenresButtonClicked()
    {
        if (_user is null)
        {
            return;
        }
        
        await GenreRatingsViewModel.ToggleExpandedAsync(UcFindUserGenreRatings, _user.Id);
    }
    
    private async Task<List<OutUserAlbumAttribute>> LoadAllUserRatings()
    {
        if (_user is null)
        {
            return [];
        }

        const int pageSize = 250;
        var result = new List<OutUserAlbumAttribute>();

        for (var skip = 0; ; skip += pageSize)
        {
            var page = await UcFindAlbumAttrs.ExecuteAsync(
                _user.Id,
                pagingOptions: new PagingOptions(Take: pageSize, Skip: skip),
                joins: new UserAlbumAttrJoinSpecification
                {
                    IncludeAlbumArtists = true
                });

            if (page.Items.Count == 0)
            {
                break;
            }

            result.AddRange(page.Items);

            if (page.Items.Count < pageSize)
            {
                break;
            }
        }

        return result;
    }
    
    private static OutGenre MapGenreBadge(OutUserGenreRating genreRating)
        => new()
        {
            Id = genreRating.Id,
            CanonicalName = genreRating.GenreName,
            ShortName = genreRating.GenreName,
            Color = genreRating.GenreColor,
            Type = GenreType.Subgenre
        };

    private void OnSearch(string text)
    {
        _searchString = text;
        _albumRatingsTable.ReloadServerData();
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_fileDownloadModule is not null)
        {
            await _fileDownloadModule.DisposeAsync();
        }
    }
    
    private string GetGenreRatingPercentText(OutUserGenreRating genreRating)
    {
        if (genreRating.WeightedPercent is null)
        {
            return "-";
        }

        return $"{genreRating.WeightedPercent.Value:+0.##;-0.##;0}%";
    }

    private string GetGenreRatingPercentColor(OutUserGenreRating genreRating)
    {
        if (genreRating.WeightedPercent is null)
        {
            return "#777777";
        }

        var normalized = Math.Clamp((genreRating.WeightedPercent.Value + 100m) * 50m, 0m, 10000m);
        return RatingHelper.GetColorForRating(normalized);
    }
}