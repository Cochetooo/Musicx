using Microsoft.AspNetCore.Components;
using MudBlazor;
using Musicx.Application.Shared.Utilities;
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
    private readonly ChartOptions _ratingOptions = new ChartOptions();

    protected override void OnParametersSet()
    {
        _donutSize = Size switch
        {
            DonutRatingSize.ExtraSmall => "48px",
            DonutRatingSize.Small => "96px",
            DonutRatingSize.Medium => "128px",
            DonutRatingSize.Large => "196px",
            _ => "128px"
        };

        _textSize = Size switch
        {
            _ => 44
        };

        _ratingColor = ColorHelper.LightenColor(
            RatingHelper.GetColorForRating(Rating),
            0.18
        );
        
        _ratingData = [(double)(Rating ?? 0), 100 - (double)(Rating ?? 0)];
        _ratingOptions.ChartPalette =
        [
            _ratingColor,
            Colors.Gray.Darken3
        ];
    }
}