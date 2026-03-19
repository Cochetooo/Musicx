using System.Text.RegularExpressions;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Artist;
using Musicx.Contracts.Dto.Responses.Specifics.Artists;
using Musicx.Contracts.Enums;

namespace Musicx.Application.Shared.Helpers;

public static class RatingHelper
{
    private static readonly List<(decimal val, string color)> Stops = new()
    {
        (0.0m,   "#CD0000"), // bordeaux
        (3000.0m,  "#FF6347"), // salmon
        (5500.0m,  "#CDCD27"), // greeny-yellow
        (6500.0m,  "#27CD27"), // green
        (7200.0m,  "#2EAD69"), // seagreen
        (8000.0m,  "#00ADAD"), // cyan
        (10000.0m, "#9500FF")  // indigo
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
            if (r <= 1500) textual = TextualRating.Unlistenable;
            else if (r <= 2500) textual = TextualRating.Terrible;
            else if (r <= 3500) textual = TextualRating.Poor;
            else if (r <= 4500) textual = TextualRating.Mediocre;
            else if (r <= 5200) textual = TextualRating.Average;
            else if (r <= 6000) textual = TextualRating.Decent;
            else if (r <= 6500) textual = TextualRating.Good;
            else if (r <= 7000) textual = TextualRating.VeryGood;
            else if (r <= 7500) textual = TextualRating.Great;
            else if (r <= 8000) textual = TextualRating.Excellent;
            else if (r <= 9000) textual = TextualRating.Outstanding;
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
            if (r <= 2000) tier = "D-";
            else if (r <= 3000) tier = "D";
            else if (r <= 3500) tier = "D+";
            else if (r <= 4000) tier = "C-";
            else if (r <= 5000) tier = "C";
            else if (r <= 5500) tier = "C+";
            else if (r <= 6000) tier = "B-";
            else if (r <= 6500) tier = "B";
            else if (r <= 7000) tier = "B+";
            else if (r <= 7500) tier = "A-";
            else if (r <= 8000) tier = "A";
            else if (r <= 8500) tier = "A+";
            else if (r <= 9000) tier = "S-";
            else if (r <= 9500) tier = "S";
            else if (r < 10000) tier = "S+";
            else if (r == 10000) tier = "S++";

            if (ratingMode == RatingMode.TierList)
            {
                return tier[0].ToString();
            }

            return tier;
        }
        
        if (ratingMode == RatingMode.Percentage)
        {
            return $"{(r/100):0.##}%";
        }

        decimal scaled;
        string format;
        int denominator;
        
        switch (ratingMode)
        {
            case RatingMode.OutOfFive:
                denominator = 5;
                scaled = r * denominator / 10000;
                format = "0.##";
                break;
            case RatingMode.OutOfTen:
                denominator = 10;
                scaled = r * denominator / 10000;
                format = "0.##";
                break;
            case RatingMode.OutOfTwenty:
                denominator = 20;
                scaled = r * denominator / 10000;
                format = "0.##";
                break;
            case RatingMode.OutOfFifty:
                denominator = 50;
                scaled = r * denominator / 10000;
                format = "0.##";
                break;
            case RatingMode.OutOfThousand:
                denominator = 1000;
                scaled = r * denominator / 10000;
                format = "0"; // pas de décimales
                break;
            case RatingMode.RatingStars:
                denominator = 5;
                scaled = r * denominator / 10000;
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
        return CalculateArtistRatingSummary(albums).Average;
    }
    
    public static OutArtistRatingStat CalculateArtistRatingSummary(IList<OutAlbum> albums)
    {
        var global = CalculateWeightedRating(albums, album => album.Stats?.Average, album => album.Stats?.Count ?? 0);
        var fans = CalculateWeightedRating(albums, album => album.Stats?.FanAverage, album => album.Stats?.FanCount ?? 0);
        var nonFans = CalculateWeightedRating(albums, album => album.Stats?.NonFanAverage, album => album.Stats?.NonFanCount ?? 0);

        return new OutArtistRatingStat
        {
            Average = global.Rating,
            Count = global.Count,
            FanAverage = fans.Rating,
            FanCount = fans.Count,
            NonFanAverage = nonFans.Rating,
            NonFanCount = nonFans.Count
        };
    }
    
    private static (decimal? Rating, long Count) CalculateWeightedRating(
        IList<OutAlbum> albums,
        Func<OutAlbum, decimal?> ratingSelector,
        Func<OutAlbum, int> countSelector)
    {
        decimal totalWeight = 0;
        decimal weightedSum = 0;
        long ratingsCount = 0;

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
            
            var rating = ratingSelector(album);
            var count = countSelector(album);

            if (rating is null || count <= 0)
            {
                continue;
            }

            weightedSum += rating.Value * coeff * count;
            totalWeight += coeff * count;
            ratingsCount += count;
        }

        return (totalWeight > 0 ? weightedSum / totalWeight : null, ratingsCount);
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
        TextualRating.Unlistenable => 500,
        TextualRating.Terrible => 1500,
        TextualRating.Poor => 3000,
        TextualRating.Mediocre => 4000,
        TextualRating.Average => 5000,
        TextualRating.Decent => 6000,
        TextualRating.Good => 6500,
        TextualRating.VeryGood => 7000,
        TextualRating.Great => 7500,
        TextualRating.Excellent => 8000,
        TextualRating.Outstanding => 9000,
        TextualRating.Masterpiece => 10000,
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