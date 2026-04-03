using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.UseCases.User.Ratings;
using Musicx.Contracts.Dto.Responses.Specifics.Ratings;

namespace Musicx.Application.Web.ViewModels.User.GenreRatings;

public sealed class UserGenreRatingsViewModel : ViewModelBase
{
    private bool _topGenresExpanded;
    private bool _topGenresFullLoaded;
    private bool _showWeighted;
    private List<OutUserGenreRating> _genreRatingsRaw = [];
    private List<OutUserGenreRating> _genreRatingsWeighted = [];

    public bool TopGenresExpanded
    {
        get => _topGenresExpanded;
        private set => SetProperty(ref _topGenresExpanded, value);
    }

    public bool TopGenresFullLoaded
    {
        get => _topGenresFullLoaded;
        private set => SetProperty(ref _topGenresFullLoaded, value);
    }

    public bool ShowWeighted
    {
        get => _showWeighted;
        set
        {
            if (SetProperty(ref _showWeighted, value))
            {
                RaisePropertyChanged(nameof(DisplayedGenreRatings));
            }
        }
    }

    public IReadOnlyList<OutUserGenreRating> GenreRatingsRaw => _genreRatingsRaw;
    public IReadOnlyList<OutUserGenreRating> GenreRatingsWeighted => _genreRatingsWeighted;

    public IReadOnlyList<OutUserGenreRating> DisplayedGenreRatings
        => ShowWeighted ? _genreRatingsWeighted : _genreRatingsRaw;

    public void Reset()
    {
        TopGenresExpanded = false;
        TopGenresFullLoaded = false;
        ShowWeighted = false;
        _genreRatingsRaw = [];
        _genreRatingsWeighted = [];
        RaisePropertyChanged(nameof(GenreRatingsRaw));
        RaisePropertyChanged(nameof(GenreRatingsWeighted));
        RaisePropertyChanged(nameof(DisplayedGenreRatings));
    }

    public async Task LoadAsync(IFindUserGenreRatingsService ratingsService, long userId, long count)
    {
        TopGenresFullLoaded = count > 5;

        var rawGenreRatings = await ratingsService.ExecuteAsync(
            userId,
            weighted: false,
            pagingOptions: new PagingOptions(Take: count, Skip: 0));

        _genreRatingsRaw = rawGenreRatings.Items;

        var weightedGenreRatings = await ratingsService.ExecuteAsync(
            userId,
            weighted: true,
            pagingOptions: new PagingOptions(Take: count, Skip: 0));

        _genreRatingsWeighted = weightedGenreRatings.Items;

        RaisePropertyChanged(nameof(GenreRatingsRaw));
        RaisePropertyChanged(nameof(GenreRatingsWeighted));
        RaisePropertyChanged(nameof(DisplayedGenreRatings));
    }

    public async Task ToggleExpandedAsync(IFindUserGenreRatingsService ratingsService, long userId, long expandedCount = 50)
    {
        TopGenresExpanded = !TopGenresExpanded;

        if (!TopGenresFullLoaded)
        {
            await LoadAsync(ratingsService, userId, expandedCount);
            TopGenresFullLoaded = true;
        }
    }
}