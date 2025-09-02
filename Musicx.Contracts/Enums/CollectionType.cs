namespace Musicx.Contracts.Enums;

[Flags]
public enum CollectionType
{
    Digital             = 1 << 0,
    Cd                  = 1 << 1,
    Dvd                 = 1 << 2,
    Vinyl               = 1 << 3,
    Cassette            = 1 << 4,
    Other               = 1 << 5,
    Wishlist            = 1 << 6,
    UsedToOwn           = 1 << 7,
}