using System.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Helpers;
using Musicx.Application.Web.Interfaces.Models.User.Ratings;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Contracts.Dto.Responses.Specifics.Ratings;
using Musicx.Contracts.Dto.Responses.User;
using Musicx.Infrastructure.API.Persistence.Specifications.User;

namespace Musicx.Presentation.Web.Client.Pages.Users;

public partial class UserView
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
    
    private List<OutUserGenreRating> _genreRatingsRaw = [];
    private List<OutUserGenreRating> _genreRatingsWeighted = [];
    private bool _showWeightedGenreRatings;

    private IReadOnlyList<OutUserGenreRating> DisplayedGenreRatings
        => _showWeightedGenreRatings ? _genreRatingsWeighted : _genreRatingsRaw;

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
        _maxRatingDistribCount = _albumRatingDistrib?.RatingCounts.Values.Max() ?? 1;
        _logger.LogInformation($"✅ Album Ratings Distribution loaded ({_user.Id})");
        
        var rawGenreRatings = await UcFindUserGenreRatings.ExecuteAsync(_user.Id, weighted: false, pagingOptions: new PagingOptions(Take: 12, Skip: 0));
        _genreRatingsRaw = rawGenreRatings.Items;

        var weightedGenreRatings = await UcFindUserGenreRatings.ExecuteAsync(_user.Id, weighted: true, pagingOptions: new PagingOptions(Take: 12, Skip: 0));
        _genreRatingsWeighted = weightedGenreRatings.Items;
        
        await _albumRatingsTable.ReloadServerData();
        await InvokeAsync(StateHasChanged);

        var bestUserRatings = await UcFindAlbumAttrs.ExecuteAsync(
            _user.Id,
            pagingOptions: new PagingOptions(Take: 6, Skip: 0),
            joins: new UserAlbumAttrJoinSpecification
            {
                IncludeAlbumArtists = true
            },
            order: new UserAlbumAttrOrderSpecification
            {
                Rating = -1,
                AlbumName = -2
            });

        _favAlbums = bestUserRatings
            .Items
            .Select(i => i.Album)
            .ToList();
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
        
        var result = await DialogService.ShowMessageBoxAsync(
            "Warning",
            "Deleting all ratings cannot be undone! Are you sure you want to delete all your ratings?",
            yesText: "Delete!", cancelText: "Cancel");

        if (result is not null && result.Value)
        {
            await UcDeleteAllRatings.ExecuteAsync(_user.Id);
            await InvokeAsync(StateHasChanged);
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
            Snackbar.Add("User not found, cannot export ratings.", Severity.Warning);
            await _exportRatingsModal.CloseAsync();
            return;
        }

        var ratings = await LoadAllUserRatings();
        if (ratings.Count == 0)
        {
            Snackbar.Add("No ratings to export.", Severity.Info);
            await _exportRatingsModal.CloseAsync();
            return;
        }

        var exportFile = UcExportRatings.Execute(ratings, _user.Name, _selectedExportFormat);
        await JS.InvokeVoidAsync(
            "fileDownload.downloadFileFromBytes",
            exportFile.FileName,
            exportFile.ContentType,
            Convert.ToBase64String(exportFile.Content));

        Snackbar.Add("Ratings exported successfully.", Severity.Success);
        await _exportRatingsModal.CloseAsync();
    }
    
    private async Task<List<OutUserAlbumAttribute>> LoadAllUserRatings()
    {
        if (_user is null)
        {
            return [];
        }

        const int pageSize = 250;
        var result = new List<OutUserAlbumAttribute>();
        var skip = 0;

        while (true)
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
            skip += pageSize;

            if (page.Items.Count < pageSize)
            {
                break;
            }
        }

        return result;
    }

    private void OnSearch(string text)
    {
        _searchString = text;
        _albumRatingsTable.ReloadServerData();
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