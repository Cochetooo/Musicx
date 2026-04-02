namespace Musicx.Application.Api.Interfaces.Caching;

/// <summary>
/// Generates stable cache keys for DataView payloads.
/// </summary>
public interface IDataViewCacheKeyFactory
{
    string BuildAlbumDataViewKey(long albumId, long? userId = null);
    string BuildArtistDataViewKey(long artistId, long? userId = null);
    string BuildGenreDataViewKey(long genreId, long? userId = null);
    string BuildUserDataViewKey(long userId, long? currentUserId = null);
}