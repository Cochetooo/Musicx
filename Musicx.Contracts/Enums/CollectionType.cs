using System.ComponentModel;

namespace Musicx.Contracts.Enums;

[Flags]
public enum CollectionType
{
    [Description("💻 Digital")]
    Digital             = 1 << 0,
    
    [Description("💿 CD")]
    Cd                  = 1 << 1,
    
    [Description("📀 DVD")]
    Dvd                 = 1 << 2,
    
    [Description("💽 Vinyl")]
    Vinyl               = 1 << 3,
    
    [Description("📼 Cassette")]
    Cassette            = 1 << 4,
    
    [Description("❓ Other")]
    Other               = 1 << 5,
    
    [Description("👀 Wishlist")]
    Wishlist            = 1 << 6,
    
    [Description("⛓️‍💥 Used to Own")]
    UsedToOwn           = 1 << 7,
}