namespace Musicx.Application.Api.Interfaces.Caching;

public interface IDataViewCacheKeyFactory
{
    string BuildArtistDataViewKey(long artistId, long? userId = null);
    string BuildGenreDataViewKey(long genreId, long? userId = null);
}