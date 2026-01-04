using System.Text.RegularExpressions;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Enums;

namespace Musicx.Application.Shared.Utilities;

public static class RatingHelper
{
    private static readonly List<(decimal val, string color)> Stops = new()
    {
        (0.0m,   "#8B0000"), // bordeaux
        (30.0m,  "#FF6347"), // salmon
        (55.0m,  "#B0B027"), // greeny-yellow
        (65.0m,  "#27C427"), // green
        (72.0m,  "#2E8D69"), // seagreen
        (80.0m,  "#008B8B"), // cyan
        (100.0m, "#4B0082")  // indigo
    };
    
    public static string GetRatingFormatted(decimal? rating, RatingMode ratingMode)
    {
        if (!rating.HasValue)
        {
            return "-";
        }

        var r = rating.Value;
        
        if (ratingMode == RatingMode.TextualShort || ratingMode == RatingMode.TextualDetailed)
        {

            TextualRating textual;
            if (r < 25) textual = TextualRating.Hate;
            else if (r < 45) textual = TextualRating.Meh;
            else if (r < 55) textual = TextualRating.Neutral;
            else if (r < 65) textual = TextualRating.Ok;
            else if (r < 75) textual = TextualRating.Good;
            else if (r < 85) textual = TextualRating.VeryGood;
            else if (r < 95) textual = TextualRating.Excellent;
            else textual = TextualRating.Favourite;

            if (ratingMode == RatingMode.TextualShort)
            {
                switch (textual)
                {
                    case TextualRating.Hate:
                    case TextualRating.Meh:
                        return "Meh";
                    case TextualRating.Neutral:
                        return "Neutral";
                    case TextualRating.Ok:
                    case TextualRating.Good:
                    case TextualRating.VeryGood:
                    case TextualRating.Excellent:
                        return "Good";
                    case TextualRating.Favourite:
                        return "Favourite";
                }
            }

            return textual.ToString().SplitCamelCase();
        }
        else if (ratingMode == RatingMode.Percentage)
        {
            return $"{r}%";
        }
        else
        {
            decimal scaled;
            string format;
            int denominator;
            
            switch (ratingMode)
            {
                case RatingMode.OutOfFive:
                    denominator = 5;
                    scaled = r * denominator / 100;
                    format = "0.##";
                    break;
                case RatingMode.OutOfTen:
                    denominator = 10;
                    scaled = r * denominator / 100;
                    format = "0.##";
                    break;
                case RatingMode.OutOfTwenty:
                    denominator = 20;
                    scaled = r * denominator / 100;
                    format = "0.##";
                    break;
                case RatingMode.OutOfFifty:
                    denominator = 50;
                    scaled = r * denominator / 100;
                    format = "0.##";
                    break;
                case RatingMode.OutOfThousand:
                    denominator = 1000;
                    scaled = r * denominator / 100;
                    format = "0"; // pas de décimales
                    break;
                case RatingMode.RatingStars:
                    denominator = 5;
                    scaled = r * denominator / 100;
                    // arrondi au 0.5 le plus proche
                    scaled = Math.Round(scaled * 2, MidpointRounding.AwayFromZero) / 2;
                    format = scaled % 1 == 0 ? "0" : "0.0";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(ratingMode), ratingMode, null);
            }

            return $"{scaled.ToString(format)} / {denominator}";
        }
    }

    public static decimal? CalculateArtistRating(IList<OutAlbum> albums)
    {
        decimal totalWeight = 0;
        decimal weightedSum = 0;

        foreach (var album in albums)
        {
            var coeff = album.ReleaseType switch
            {
                ReleaseType.Lp => 1.0m,
                ReleaseType.Soundtrack => 0.8m,
                ReleaseType.MixTape => 0.7m,
                ReleaseType.Ep => 0.6m,
                ReleaseType.DjMix => 0.5m,
                ReleaseType.Covers => 0.4m,
                ReleaseType.Single => 0.3m,
                ReleaseType.Remix => 0.2m,
                _ => 0.15m
            };

            if (album.Stats is null)
            {
                continue;
            }
            
            weightedSum += album.Stats.Average * coeff * album.Stats.Count;
            totalWeight += coeff * album.Stats.Count;
        }
        
        return totalWeight > 0 ? weightedSum / totalWeight : null;
    }

    public static string GetColorForRating(decimal? rating)
    {
        if (rating is null)
        {
            return "#77777777";
        }

        for (int i = 0; i < Stops.Count - 1; i++)
        {
            var a = Stops[i];
            var b = Stops[i + 1];

            if (rating >= a.val && rating <= b.val)
            {
                var t = (rating.Value - a.val) / (b.val - a.val);
                return ColorHelper.Interpolate(a.color, b.color, (double)t);
            }
        }

        return Stops[^1].color;
    }
    
    public static int ToInt(this TextualRating r) => r switch
    {
        TextualRating.Hate => 15,
        TextualRating.Meh => 35,
        TextualRating.Neutral => 50,
        TextualRating.Ok => 60,
        TextualRating.Good => 70,
        TextualRating.VeryGood => 80,
        TextualRating.Excellent => 90,
        TextualRating.Favourite => 100,
        _ => 0
    };

    public static decimal GetDecimalStep(this RatingMode r) => r switch
    {
        RatingMode.OutOfFive => .5m,
        RatingMode.OutOfTen => .5m,
        RatingMode.OutOfTwenty => .5m,
        RatingMode.OutOfFifty => 1,
        RatingMode.OutOfThousand => 1,
        RatingMode.Percentage => 1,
        RatingMode.RatingStars => .5m,
        _ => 1
    };

}