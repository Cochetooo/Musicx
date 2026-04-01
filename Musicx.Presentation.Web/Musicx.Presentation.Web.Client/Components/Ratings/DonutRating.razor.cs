using Microsoft.AspNetCore.Components;
using Musicx.Application.Shared.Helpers;
using Musicx.Application.Shared.Styling;
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
    
    private string _ratingColor = string.Empty;
    private string _trackColor = string.Empty;

    private double _radius, _strokeWidth, _circumference, _dashOffset;

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
            0.2
        );
        _trackColor = UiColorPalette.Rating.Track;

        _strokeWidth = Size switch
        {
            DonutRatingSize.ExtraSmall => 9,
            DonutRatingSize.Small => 10,
            DonutRatingSize.Medium => 11,
            DonutRatingSize.Large => 12,
            _ => 10
        };

        _radius = 50 - (_strokeWidth / 2.0) - 1.0;
        _circumference = 2 * Math.PI * _radius;

        var normalizedRating = Math.Clamp((double)(Rating ?? 0) / 10000d, 0d, 1d);
        _dashOffset = _circumference * (1d - normalizedRating);
    }
}