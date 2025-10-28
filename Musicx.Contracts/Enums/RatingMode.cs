using System.ComponentModel;

namespace Musicx.Contracts.Enums;

public enum RatingMode
{
    [Description("⭐ /5")]
    OutOfFive = 0,
    
    [Description("⭐ /10")]
    OutOfTen = 1,
    
    [Description("⭐ /20")]
    OutOfTwenty = 2,
    
    [Description("⭐ /50")]
    OutOfFifty = 3,
    
    [Description("⭐ /1000")]
    OutOfThousand = 4,
    
    [Description("➗ 100%")]
    Percentage = 5,
    
    [Description("⭐⭐⭐⭐⭐")]
    RatingStars = 6,
    
    [Description("🏷️ Tags Simplified")]
    TextualShort = 7,
    
    [Description("🏷️ Tags Detailed")]
    TextualDetailed = 8
}