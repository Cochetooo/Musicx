using Microsoft.AspNetCore.Components;
using MudBlazor;
using Musicx.Application.Shared.Helpers;
using Musicx.Contracts.Enums;

namespace Musicx.Presentation.Web.Client.Components.Ratings;

public enum DonutRatingSize
{
    ExtraSmall,
    Small,
    Medium,
    Large
}

public partial class DonutRating
{
    [Parameter] public DonutRatingSize Size { get; set; } = DonutRatingSize.Medium;
    [Parameter] public RatingMode RatingMode { get; set; }
    [Parameter, EditorRequired] public decimal? Rating { get; set; }

    private string _donutSize = string.Empty;
    private int _textSize;

    private double[] _ratingData = [];
    private string _ratingColor = string.Empty;
    private readonly ChartOptions _ratingOptions = new();

    protected override void OnParametersSet()
    {
        _donutSize = Size switch
        {
            DonutRatingSize.ExtraSmall => "36px",
            DonutRatingSize.Small => "56px",
            DonutRatingSize.Medium => "96px",
            DonutRatingSize.Large => "128px",
            _ => "84px"
        };

        _textSize = Size switch
        {
            _ => 44
        };

        _ratingColor = ColorHelper.LightenColor(
            RatingHelper.GetColorForRating(Rating),
            0.22
        );
        
        _ratingData = [(double)(Rating ?? 0), 100 - (double)(Rating ?? 0)];
        _ratingOptions.ChartPalette =
        [
            _ratingColor,
            Colors.Gray.Darken3
        ];
        _ratingOptions.ShowToolTips = false;
    }
}