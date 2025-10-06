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

            return SplitCamelCase(textual.ToString());
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

    public static string GetColorForRating(decimal? rating)
    {
        if (rating is null)
        {
            return "#777777";
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
    
    private static string SplitCamelCase(string input)
        => Regex.Replace(input, "([a-z])([A-Z])", "$1 $2");
}