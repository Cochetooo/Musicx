using System.Text.RegularExpressions;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Enums;

namespace Musicx.Application.Shared.Helpers;

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
            if (r <= 15) textual = TextualRating.Unlistenable;
            else if (r <= 25) textual = TextualRating.Terrible;
            else if (r <= 35) textual = TextualRating.Poor;
            else if (r <= 45) textual = TextualRating.Mediocre;
            else if (r <= 52) textual = TextualRating.Average;
            else if (r <= 60) textual = TextualRating.Decent;
            else if (r <= 65) textual = TextualRating.Good;
            else if (r <= 70) textual = TextualRating.VeryGood;
            else if (r <= 75) textual = TextualRating.Great;
            else if (r <= 80) textual = TextualRating.Excellent;
            else if (r <= 90) textual = TextualRating.Outstanding;
            else textual = TextualRating.Masterpiece;

            if (ratingMode == RatingMode.TextualShort)
            {
                switch (textual)
                {
                    case TextualRating.Unlistenable:
                    case TextualRating.Terrible:
                    case TextualRating.Poor:
                        return "Poor";
                    case TextualRating.Mediocre:
                    case TextualRating.Average:
                    case TextualRating.Decent:
                        return "Average";
                    case TextualRating.Good:
                    case TextualRating.VeryGood:
                        return "Good";
                    case TextualRating.Great:
                    case TextualRating.Excellent:
                    case TextualRating.Outstanding:
                        return "Excellent";
                    case TextualRating.Masterpiece:
                        return "Masterpiece";
                }
            }

            return textual.ToString().SplitCamelCase();
        }
        
        if (ratingMode == RatingMode.TierList || ratingMode == RatingMode.TierListDetailed)
        {
            var tier = string.Empty;
            if (r <= 20) tier = "D-";
            else if (r <= 30) tier = "D";
            else if (r <= 35) tier = "D+";
            else if (r <= 40) tier = "C-";
            else if (r <= 50) tier = "C";
            else if (r <= 55) tier = "C+";
            else if (r <= 60) tier = "B-";
            else if (r <= 65) tier = "B";
            else if (r <= 70) tier = "B+";
            else if (r <= 75) tier = "A-";
            else if (r <= 80) tier = "A";
            else if (r <= 85) tier = "A+";
            else if (r <= 90) tier = "S-";
            else if (r <= 95) tier = "S";
            else if (r < 100) tier = "S+";
            else if (r == 100) tier = "S++";

            if (ratingMode == RatingMode.TierList)
            {
                return tier[0].ToString();
            }

            return tier;
        }
        
        if (ratingMode == RatingMode.Percentage)
        {
            return $"{r:0.##}%";
        }

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

        return $"{scaled.ToString(format)}";
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

    public static bool IsNumeric(this RatingMode ratingMode)
        => ratingMode == RatingMode.OutOfFive || ratingMode == RatingMode.OutOfTen 
            || ratingMode == RatingMode.OutOfTwenty || ratingMode == RatingMode.OutOfFifty
            || ratingMode == RatingMode.OutOfThousand;

    public static int? GetDenominator(this RatingMode ratingMode)
        => ratingMode switch
        {
            RatingMode.OutOfFive => 5,
            RatingMode.OutOfTen => 10,
            RatingMode.OutOfTwenty => 20,
            RatingMode.OutOfFifty => 50,
            RatingMode.OutOfThousand => 1000,
            RatingMode.Percentage => 100,
            _ => null
        };
    
    public static int ToInt(this TextualRating r) => r switch
    {
        TextualRating.Unlistenable => 5,
        TextualRating.Terrible => 15,
        TextualRating.Poor => 30,
        TextualRating.Mediocre => 40,
        TextualRating.Average => 50,
        TextualRating.Decent => 60,
        TextualRating.Good => 65,
        TextualRating.VeryGood => 70,
        TextualRating.Great => 75,
        TextualRating.Excellent => 80,
        TextualRating.Outstanding => 90,
        TextualRating.Masterpiece => 100,
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